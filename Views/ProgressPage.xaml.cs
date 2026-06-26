using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WinUI_installer.ViewModels;

namespace WinUI_installer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProgressPage : Page
    {
        private InstallerViewModel ViewModel { get; set; }
        public ProgressPage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetService<InstallerViewModel>();
            Loaded += ProgressPage_Loaded;
            DataContext = ViewModel;
        }

        private async void ProgressPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InstallFiles();
            Frame.Navigate(typeof(FinishPage));
        }

        private async void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
