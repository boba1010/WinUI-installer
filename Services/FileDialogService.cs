using System.Threading.Tasks;
using WinUI_installer.Views;

namespace WinUI_installer.Services
{
    public class FileDialogService : IFileDialogService
    {
        public async Task<string?> PickFolderAsync()
        {
            var dialog = new FolderPickerDialog();
            var result = await dialog.ShowAsync();

            return dialog.SelectedPath;
        }
    }
}
