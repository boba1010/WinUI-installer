namespace WinUI_installer.Models
{
    public class InstallData
    {
        public string AppName { get; set; }
        public string AppExeName { get; set; }
        public string InstallPath { get; set; }
        public IProgress<InstallProgress> Progress { get; set; }
        public bool CreateUninstallBat { get; set; }
        public bool LaunchAfterInstall { get; set; }
        public bool CreateShortcut { get; set; }
    }
}
