using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using WinUI_installer.Models;
using WinUI_installer.ViewModels;

namespace WinUI_installer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinishPage : Page
    {
        private InstallerViewModel ViewModel { get; set; }
        public FinishPage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetService<InstallerViewModel>();
            DataContext = ViewModel;
        }
    }
}
