using System.Threading.Tasks;

namespace WinUI_installer.Services
{
    public interface IFileDialogService
    {
        public Task<string?> PickFolderAsync();
    }
}
