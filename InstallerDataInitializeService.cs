namespace WinUI_installer
{
    public class InstallerDataInitializeService
    {

        public static string AppName { get; private set; } = "UMT";
        public static string AppExeName { get; private set; } = "hello.exe";
        public static string DefaultPath { get; set; } = 
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                AppName);

        public static async Task<InstallData> PrepareInstallerDataAsync(
            bool createShortcut, 
            bool createUninstallBat,
            IProgress<InstallProgress> progress = null)
        {
            // WARNING: IT IS RECOMMENDED TO CHANGE ONLY "AppName" AND "AppExeName"
            
            return new InstallData
            {
                AppExeName = AppExeName,
                AppName = AppName,
                InstallPath = DefaultPath,
                Progress = progress,
                CreateShortcut = createShortcut,
                CreateUninstallBat = createUninstallBat,
            };
        }
    }
}
