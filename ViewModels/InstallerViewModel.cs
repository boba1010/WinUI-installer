using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using WinUI_installer.InstallerCore;
using WinUI_installer.Models;
using WinUI_installer.Services;

namespace WinUI_installer.ViewModels
{
    public partial class InstallerViewModel(IFileDialogService fileDialogService, PrepareInstallerAPI prepareInstallerAPI) : ObservableObject
    {
        [ObservableProperty]
        public partial bool CreateShortcut { get; set; } = true;

        [ObservableProperty]
        public partial bool LaunchAfterInstall { get; set; }

        [ObservableProperty]
        public partial bool CreateUninstallBat { get; set; } = true;

        [ObservableProperty]
        public partial string FolderPath { get; set; } = "C:\\Program Files\\App";

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

        private InstallerEngine installer = new();
        private InstallData InstallData { get; set; }
        public async Task PrepareFiles()
        {
            Progress<InstallProgress> progress = new Progress<InstallProgress>(update =>
            {
                StatusMessage = update.StatusMessage;
                ProgressPrecentage = update.ProgressPrecentage;
            });

            InstallData = await prepareInstallerAPI.PrepareInstallerDataAsync(
                createShortcut: CreateShortcut,
                createUninstallBat: CreateUninstallBat,
                launchAfterInstall: LaunchAfterInstall,
                progress: progress);
            await installer.PrepareAsync(InstallData);
        }

        [RelayCommand]
        public void InstallEnd()
        {
            if (InstallData.LaunchAfterInstall)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(InstallData.InstallPath, InstallData.AppExeName),
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
