using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace curs.ViewModels.LoginRegistration
{
    public class ErrorMessageViewModel : BaseViewModel, INavigationAware
    {
        #region Fields and Properties

        private string textMessage;
        public string ErrorMessage
        {
            get
            {
                return textMessage;
            }
            set
            {
                Set(ref textMessage, value);
            }
        }

        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public ErrorMessageViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;

            GoToRegistration = new DelegateCommand(ExecuteGoToRegistration, CanGoToRegistration);
            GoToLogin = new DelegateCommand(ExecuteGoToLogin, CanGoToLogin);
        }
        #endregion

        #region Methods


        //Command для кнопки Далее------------------------------------
        public ICommand GoToRegistration { get; }

        private void ExecuteGoToRegistration(object obj)
        {
            navigationManager.Navigate(NavigationKeys.RegisterViewKey);
        }

        private bool CanGoToRegistration(object obj)
        {
            return true;
        }
        //----------------------------------------------------------

        //Command для кнопки Вход------------------------------------
        public ICommand GoToLogin { get; }

        private void ExecuteGoToLogin(object obj)
        {
            navigationManager.Navigate(NavigationKeys.LoginViewKey);
        }

        private bool CanGoToLogin(object obj)
        {
            return true;
        }
        //----------------------------------------------------------


        public void WantDoSomethingBeforeClose()
        {
            
        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            ErrorMessage = obj as string ?? "Ошибка!";
        }
        #endregion
    }
}
