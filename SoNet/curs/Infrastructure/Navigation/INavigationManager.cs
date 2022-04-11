

namespace curs.Infrastructure.Navigation
{
    public interface INavigationManager
    {
        void Navigate(string navigationKey, object obj = null);
        void Register(string navigationKey, INavigationManager navigationManager = null);

        string AddImage();
        string AddFile();
        string DownloadFile();
    }
}
