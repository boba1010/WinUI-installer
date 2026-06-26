using Microsoft.Win32;
using ShellLink;
using ShellLink.Flags;
using ShellLink.Structures;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Principal;
using System.Threading.Tasks;
using WinUI_installer.Models;

namespace WinUI_installer.InstallerCore
{
    public class InstallerEngine
    {
        private InstallData InstallData { get; set; }

        public async Task PrepareAsync(InstallData installData)
        {
            var isElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);

            if (!isElevated)
            {
                installData.Progress.Report(new() { ProgressPrecentage = 100, StatusMessage = "Installer requires administrator privileges. Please run as admin." });
                throw new InvalidOperationException("Installer requires administrator privileges. Please run as admin.");
            }

            InstallData = installData;

            // 1. Check if bundled app resource exists
            var assembly = typeof(App).Assembly;
            var resourceName = "WinUI_installer.Embedded.app.zip"; // Your embedded resource name
            var resourceStream = assembly.GetManifestResourceStream(resourceName);

            if (resourceStream == null)
                throw new FileNotFoundException($"Bundled app resource not found: {resourceName}");

            // 2. Check if the directory is writable
            try
            {
                var testFile = Path.Combine(InstallData.InstallPath, ".test");
                Directory.CreateDirectory(InstallData.InstallPath);
                System.IO.File.WriteAllText(testFile, "");
                System.IO.File.Delete(testFile);
            }
            catch
            {
                throw new UnauthorizedAccessException($"No write permission to {InstallData.InstallPath}");
            }

            var drive = new DriveInfo(Path.GetPathRoot(InstallData.InstallPath));
            if (drive.AvailableFreeSpace < 500_000_000) // 500MB
                throw new IOException("Not enough disk space");
        }
        
        public async Task InstallAsync()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            InstallData.Progress.Report(new() { StatusMessage = "Creating root folder...", ProgressPrecentage = 1 });

            try
            {
                var assembly = typeof(App).Assembly;
                var resourceName = "WinUI_installer.Embedded.app.zip";
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    InstallData.Progress?.Report(new InstallProgress { StatusMessage = "Extracting bundled app...", ProgressPrecentage = 10 });
                    using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
                    zip.ExtractToDirectory(tempPath);
                }

                // 2. Copy from temp to Program Files
                if (Directory.Exists(InstallData.InstallPath))
                    Directory.Delete(InstallData.InstallPath, recursive: true); // Clean old install

                InstallData.Progress?.Report(new InstallProgress { StatusMessage = "Copying files to the chosen directory...", ProgressPrecentage = 40 });
                CopyDirectory(tempPath, InstallData.InstallPath);

                InstallData.Progress?.Report(new InstallProgress { StatusMessage = "Writing registry entry...", ProgressPrecentage = 80 });
                WriteUninstallRegistry();

                if (InstallData.CreateUninstallBat)
                {
                    InstallData.Progress?.Report(new() { StatusMessage = "Creating 'uninstall.bat'...", ProgressPrecentage = 85 });
                    CreateUninstallFile();
                }

                if (InstallData.CreateShortcut)
                {
                    InstallData.Progress?.Report(new() { StatusMessage = "Creating shortcuts...", ProgressPrecentage = 90 });
                    await CreateShortcutsAsync();
                }

                await CleanupAsync();

                InstallData.Progress?.Report(new InstallProgress { StatusMessage = "Installation complete", ProgressPrecentage = 100 });

            }
            catch (Exception ex)
            {
                InstallData.Progress?.Report(new InstallProgress { StatusMessage = $"Error: {ex.Message}", ProgressPrecentage = 100 });
            }
            finally
            {
                // Cleanup temp
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, recursive: true);
            }
        }

        private async Task CleanupAsync()
        {
            var PID = Environment.ProcessId;

            var batchFile = Path.Combine(Path.GetTempPath(), "cleanup.bat");
            File.WriteAllText(batchFile, $@"
@echo off
set PID={PID}
:wait
tasklist /fi ""PID eq %PID%"" | find /I ""%PID%"" >nul
if %errorlevel% equ 0 (
    timeout /t 1 /nobreak
    goto wait
)
rmdir /s /q ""C:\Users\%username%\AppData\Local\Temp\ReligiousQuizCompetitionInstallerTemp""
del ""%~f0""
");

            Process.Start(new ProcessStartInfo
            {
                FileName = batchFile,
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

        private void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var file in Directory.GetFiles(source))
            {
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
            }

            foreach (var dir in Directory.GetDirectories(source))
            {
                CopyDirectory(dir, Path.Combine(destination, Path.GetDirectoryName(dir)));
            }
        }

        private void CreateUninstallFile()
        {
            var uninstallBat = @$"
@echo off
(
echo Starting uninstall...
timeout /t 1 /nobreak
rmdir /s /q ""%~dp0""

del ""%USERPROFILE%\Desktop\{InstallData.AppName}.lnk""
del ""%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\{InstallData.AppName}.lnk""

echo About to delete registry key for: {InstallData.AppName}
reg delete ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{InstallData.AppName}"" /f
echo Error level: %errorlevel%
) > ""C:\temp\uninstall_log.txt"" 2>&1
del ""%~f0""
";

            File.WriteAllText(Path.Combine(InstallData.InstallPath, "uninstall.bat"), uninstallBat);
        }

        private void WriteUninstallRegistry()
        {
            var key = Registry.LocalMachine.OpenSubKey(
                     @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", writable: true);

            var appKey = key.CreateSubKey(InstallData.AppName);
            appKey.SetValue("DisplayName", InstallData.AppName);
            appKey.SetValue("InstallLocation", InstallData.InstallPath);
            appKey.SetValue("UninstallString", $"\"{Path.Combine(InstallData.InstallPath, "uninstall.bat")}\"");
            appKey.SetValue("DisplayVersion", "1.0.0");  // Your version
            appKey.SetValue("Publisher", "Boba");  // Optional
            appKey.SetValue("NoModify", 1);  // Don't allow modify
            appKey.SetValue("NoRepair", 1);  // Don't allow repair
        }

        public async Task CreateShortcutsAsync()
        {
            var appExePath = Path.Combine(InstallData.InstallPath, InstallData.AppExeName);

            var startMenuPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                "Programs",
                InstallData.AppName);

            Directory.CreateDirectory(startMenuPath);
            CreateShortcut(appExePath, Path.Combine(startMenuPath, $"{InstallData.AppName}.lnk"));

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            CreateShortcut(appExePath, Path.Combine(desktopPath, $"{InstallData.AppName}.lnk"));
        }

        private void CreateShortcut(string targetPath, string shortcutPath)
        {
            Shortcut shortcut = Shortcut.CreateShortcut(targetPath);

            shortcut.StringData ??= new StringData();

            shortcut.ShellLinkHeader.LinkFlags |= LinkFlags.HasWorkingDir | LinkFlags.HasRelativePath;

            shortcut.StringData.RelativePath = Path.GetFileName(targetPath);
            shortcut.StringData.WorkingDir = Path.GetDirectoryName(targetPath);

            shortcut.WriteToFile(shortcutPath);
        }
    }
}
