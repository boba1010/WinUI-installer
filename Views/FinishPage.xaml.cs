namespace WinUI_installer.Views
{
    public sealed partial class FinishPage : Page
    {
        private InstallerViewModel? ViewModel { get; set; }
        public FinishPage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetService<InstallerViewModel>();
            DataContext = ViewModel;
        }
    }
}
