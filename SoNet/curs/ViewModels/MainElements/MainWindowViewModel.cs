using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;

namespace curs.ViewModels.MainElements
{
    public class MainWindowViewModel : BaseViewModel
    {   
        private readonly INavigationManager navigationManager;

        public MainWindowViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;

            //При запуске программы сразу произойдет вставка в контент ГЛАВНОГО окна одного из ГЛАВНЫХ user control 
            this.navigationManager.Navigate(NavigationKeys.ContextLoginRegisterKey);   
        }
    }
}
