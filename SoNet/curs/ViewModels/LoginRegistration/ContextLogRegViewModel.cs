using curs.Infrastructure.Navigation;
using curs.View.LoginRegistrationView;
using curs.View.MainElements;
using curs.View.RightInformationView.Admin;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace curs.ViewModels.LoginRegistration
{
    public class ContextLogRegViewModel : BaseViewModel, INavigationAware, IMainUserControl
    {
        #region Fields

        //Менеджер перехода к Логину
        private readonly INavigationManager navigationManager;

        #endregion

        #region Ctors

        public ContextLogRegViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        #endregion

        #region Methods

        //Интерфейс INavigationAware----------------------------------------------------------------------------
        public void WantDoSomethingBeforeClose()
        {
            
        }

        public void WantDoSomethingBeforeOpen(object obj= null)
        {
            try
            {
                object[] array = obj as object[];
                if (array == null)
                    return;
                var dispatcher = array[0] as Dispatcher;
                var view = array[1] as ContentForLoginRegistration;

                NavigationManager localNavigationManager = new NavigationManager(dispatcher, view.LoginRegistrationContent);           //Создание локального менеджера с контентом главного UserControl для Логина и Регистрации

                //Регистрация user controls который будут вставляться в контент главного UserControl для Логина и Регистрации-------------------------------------------------------------------------------------------------------------------------
                localNavigationManager.Register(NavigationKeys.LoginViewKey, navigationManager);
                localNavigationManager.Register(NavigationKeys.RegisterViewKey);
                localNavigationManager.Register(NavigationKeys.RegisterEndViewKey);
                localNavigationManager.Register(NavigationKeys.RegisterPictureViewKey, navigationManager);
                localNavigationManager.Register(NavigationKeys.ErrorMessageViewKey);
                //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                

                localNavigationManager.Navigate(NavigationKeys.LoginViewKey);           //Переход к окну Логина
            }
            catch
            { 

            }
        }
        //------------------------------------------------------------------------------------------------------

        //Реализация интерфейса IMainUserControl
        public void MainUserControl()
        {

        }
        
        #endregion
    }
}
