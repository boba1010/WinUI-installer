namespace WinUI_installer.Views
{
    public sealed partial class ProgressPage : Page
    {
        private InstallerViewModel? ViewModel { get; set; }
        public ProgressPage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetService<InstallerViewModel>();
            Loaded += ProgressPage_Loaded;
            DataContext = ViewModel;
        }

        private async void ProgressPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
                return;
            await ViewModel.InstallFiles();
            Frame.Navigate(typeof(FinishPage));
        }

        private async void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
