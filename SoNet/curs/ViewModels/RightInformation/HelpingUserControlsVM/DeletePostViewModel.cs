using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using Models;
using System.Windows.Input;
using DatabaseClasses.UnitOfWorkPattern;

namespace curs.ViewModels.RightInformation.HelpingUserControlsVM
{
    class DeletePostViewModel : BaseViewModel, INavigationAware
    {
        #region Fields and Properties
        private string emailCurrentUser;
        private Post deleteElement;

        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public DeletePostViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;

            GoToProfile = new DelegateCommand(ExecuteGoToProfile, CanGoToProfile);
            Yes = new DelegateCommand(ExecuteYes, CanYes);
            No = new DelegateCommand(ExecuteNo, CanNo);
        }
        #endregion


        #region Methods

        //Command для кнопки Назад---------------------------------------------------
        public ICommand GoToProfile { get; }

        private void ExecuteGoToProfile(object obj)
        {
            navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);         //Переход к user control Профиль
        }
        private bool CanGoToProfile(object obj)
        {
            return true;
        }
        //------------------------------------------------------------------------------------

        //Command для кнопки Назад---------------------------------------------------
        public ICommand Yes { get; }

        private void ExecuteYes(object obj)
        {
            using(UnitOfWork unit = new UnitOfWork())
            {
                unit.PostRepos.DeleteItem(new object[] { deleteElement.Id });
                unit.Save();
            }
            navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);         //Переход к user control Профиль
        }
        private bool CanYes(object obj)
        {
            return true;
        }

        //Command для кнопки Назад---------------------------------------------------
        public ICommand No { get; }

        private void ExecuteNo(object obj)
        {
            navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);         //Переход к user control Профиль
        }
        private bool CanNo(object obj)
        {
            return true;
        }
        //------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------
        //Реализация интерфейса INavigationAware--------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            var arrayInfo = obj as object[];
            if (arrayInfo == null)
                return;

            emailCurrentUser = arrayInfo[0] as string;
            deleteElement = arrayInfo[1] as Post;
        }
        //----------------------------------------------------------------------
        #endregion
    }

}
