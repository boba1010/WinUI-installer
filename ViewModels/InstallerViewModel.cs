using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using WinUI_installer.InstallerCore;
using WinUI_installer.Services;

namespace WinUI_installer.ViewModels
{
    public partial class InstallerViewModel(IFileDialogService fileDialogService) : ObservableObject
    {
        private static InstallData InstallData { get; set; }

        [ObservableProperty]
        public partial bool CreateShortcut { get; set; } = true;

        [ObservableProperty]
        public partial bool LaunchAfterInstall { get; set; }

        [ObservableProperty]
        public partial bool CreateUninstallBat { get; set; } = true;

        [ObservableProperty]
        public partial string FolderPath { get; set; } = InstallerDataInitializeService.DefaultPath;
        partial void OnFolderPathChanged(string value)
        {
            InstallerDataInitializeService.DefaultPath = value;
        }

        [ObservableProperty]
        public partial Language Language { get; set; }

        public string[] Languages { get; } = Enum.GetNames<Language>();

        [ObservableProperty]
        public partial int ProgressPrecentage { get; set; }

        [ObservableProperty]
        public partial string StatusMessage { get; set; }

        [RelayCommand]
        public async Task BrowseFolders()
        {
            FolderPath = await fileDialogService.PickFolderAsync();
        }

        private readonly InstallerEngine installer = new();
        
        public async Task PrepareFiles()
        {
            Progress<InstallProgress> progress = new(update =>
            {
                StatusMessage = update.StatusMessage;
                ProgressPrecentage = update.ProgressPrecentage;
            });

            InstallData = await InstallerDataInitializeService.PrepareInstallerDataAsync(
                createShortcut: CreateShortcut,
                createUninstallBat: CreateUninstallBat,
                progress: progress);
            await installer.PrepareAsync(InstallData);
        }

        [RelayCommand]
        public void EndInstall()
        {
            if (LaunchAfterInstall)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(InstallData.InstallPath, InstallData.AppExeName),
                    CreateNoWindow = false,
                    CreateNewProcessGroup = true,
                });
            }
            Environment.Exit(0);
        }

        public async Task InstallFiles()
        {
            await Task.Run(async () =>
            {
                await installer.InstallAsync();
            });
        }
    }

    public enum Language
    {
        English,
        Arabic,
    }
}
