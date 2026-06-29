namespace WinUI_installer.Views
{
    public sealed partial class InstallLocationPage : Page
    {
        private InstallerViewModel? ViewModel {  get; set; }

        public InstallLocationPage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetService<InstallerViewModel>();
            DataContext = ViewModel;
        }

        private async void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void OnInstallButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
                return;

            await ViewModel.PrepareFiles();
            Frame.Navigate(typeof(ProgressPage));
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
