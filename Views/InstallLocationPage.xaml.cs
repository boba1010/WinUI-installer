using Microsoft.Extensions.DependencyInjection;
using WinUI_installer.ViewModels;

namespace WinUI_installer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallLocationPage : Page
    {
        private InstallerViewModel ViewModel {  get; set; }

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
            await ViewModel.PrepareFiles();
            Frame.Navigate(typeof(ProgressPage));
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
