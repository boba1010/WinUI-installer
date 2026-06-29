using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using WinUI_installer.InstallerCore;
using WinUI_installer.Services;
using WinUI_installer.ViewModels;

namespace WinUI_installer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public Window Window { get; private set; }

        public static IServiceProvider Services { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            var services = new ServiceCollection();

            ConfigureServices(services);

            Services = services.BuildServiceProvider();
        }

        void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<InstallerViewModel>();
            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<MainWindow>();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Window = new MainWindow();
            Window.Activate();
        }
    }
}
