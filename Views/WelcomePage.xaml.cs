using Microsoft.Extensions.DependencyInjection;
using WinUI_installer.ViewModels;

namespace WinUI_installer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        private InstallerViewModel ViewModel { get; set; }

        public WelcomePage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetService<InstallerViewModel>();
            DataContext = ViewModel;
        }

        private async void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InstallLocationPage));
        }

        private async void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
