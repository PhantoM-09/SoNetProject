using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation.HelpingUserControlsVM
{
    class ExitViewModel : BaseViewModel, INavigationAware
    {
        #region Fields and Properties

        private string emailCurrentUser;
        private NavigationManager navigationManager;
        NavigationManager globalNavigationManager;

        #endregion

        #region Ctors
        public ExitViewModel(NavigationManager navigationManager, NavigationManager globalNavigationManager)
        {
            this.navigationManager = navigationManager;
            this.globalNavigationManager = globalNavigationManager;

            GoToProfile = new DelegateCommand(ExecuteGoToProfile, CanGoToProfile);
            GoToLogin = new DelegateCommand(ExecuteGoToLogin, CanGoToLogin);
        }
        #endregion


        #region Methods

        //----------------------------------------------------------


        //Command для кнопки Нет(переход в Профиль)---------------------------------------------------
        public ICommand GoToProfile { get; }

        private void ExecuteGoToProfile(object obj)
        {
            navigationManager.Register(NavigationKeys.ProfileViewKey);
            navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);
        }
        private bool CanGoToProfile(object obj)
        {
            return true;
        }

        //--------------------------------------------------------



        //Command для кнопки Да(переход в Логин)---------------------------------------------------
        public ICommand GoToLogin { get; }

        private void ExecuteGoToLogin(object obj)
        {
            using (UnitOfWork unit = new UnitOfWork())
            {
                string email = globalNavigationManager.EmailCurrentUser;
                var user = unit.UserDataRepos.GetItem(new object[] { email });          //Достаем юзера, который сейчас в сети
                user.IsOnline = false;                                                  //Меняем ему статус,  как он выходит

                unit.UserDataRepos.UpdateItem(user);
                unit.Save();
            }
            globalNavigationManager.Register(NavigationKeys.ContextLoginRegisterKey);
            globalNavigationManager.Navigate(NavigationKeys.ContextLoginRegisterKey);  
        }
        private bool CanGoToLogin(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        //Реализация интерфейса INavigationAware--------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            var email = obj as string;
            if (email == null)
                return;

            emailCurrentUser = email;
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
