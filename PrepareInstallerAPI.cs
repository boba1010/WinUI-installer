using System.Threading.Tasks;
using WinUI_installer.Models;

namespace WinUI_installer
{
    public class PrepareInstallerAPI
    {
        public async Task<InstallData> PrepareInstallerDataAsync(
            bool createShortcut, 
            bool createUninstallBat, 
            bool launchAfterInstall, 
            IProgress<InstallProgress> progress = null)
        {
            // WARNING: IT IS RECOMMENDED TO CHANGE ONLY "appName" AND "appExeName"
            const string AppName = "ReligiousQuizCompetition";
            const string AppExeName = "ReligiousQuizCompetitionWinUI.exe";

            return new InstallData
            {
                AppExeName = AppExeName,
                AppName = AppName,
                InstallPath = $"C:\\Program Files\\{AppName}",
                Progress = progress,
                CreateShortcut = createShortcut,
                CreateUninstallBat = createUninstallBat,
                LaunchAfterInstall = launchAfterInstall,
            };
        }
    }
}
