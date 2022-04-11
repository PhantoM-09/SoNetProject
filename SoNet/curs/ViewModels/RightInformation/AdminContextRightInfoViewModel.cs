using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.View.MainElements;
using curs.View.RightInformationView;
using curs.View.RightInformationView.Admin;
using curs.View.RightInformationView.HelpingUserControls;
using curs.View.Settings;
using curs.ViewModels.Base;
using curs.ViewModels.RightInformation.HelpingUserControlsVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace curs.ViewModels.RightInformation
{
    class AdminContextRightInfoViewModel : BaseViewModel, INavigationAware, IMainUserControl
    {
        #region Fields and Property


        private string emailCurrentUser;
        private readonly INavigationManager navigationManager;
        private INavigationManager localNavigationManager;
        #endregion


        #region Ctors

        public AdminContextRightInfoViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;

            GoToDataBase = new DelegateCommand(ExecuteGoToDataBase, CanGoToDataBase);
            GoToExit = new DelegateCommand(ExecuteGoToExit, CanGoToExit);
        }

        #endregion


        #region Methods

        //Command для кнопки База Данных---------------------------------------------------
        public ICommand GoToDataBase { get; }

        private void ExecuteGoToDataBase(object obj)
        {
            localNavigationManager.Register(NavigationKeys.DataBaseViewKey);
            localNavigationManager.Navigate(NavigationKeys.DataBaseViewKey, emailCurrentUser);           //Переход к окну Базы данных
        }
        private bool CanGoToDataBase(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        //Command для кнопки Выход-----------------------------------
        public ICommand GoToExit { get; }

        private void ExecuteGoToExit(object obj)
        {
            localNavigationManager.Register(NavigationKeys.ExitViewKey, navigationManager);
            localNavigationManager.Navigate(NavigationKeys.ExitViewKey, emailCurrentUser);             //Переход к user control Выход
        }
        private bool CanGoToExit(object obj)
        {
            return true;
        }
        //-----------------------------------------------------------



        //Интерфейс INavigationAware----------------------------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {

            try
            {
                object[] array = obj as object[];
                if (array == null)
                    return;
                var dispatcher = array[0] as Dispatcher;
                var view = array[1] as AdminContentForRightInfo;
                emailCurrentUser = array[2] as string;

                NavigationManager localNavigationManager = new NavigationManager(dispatcher, view.RightInformationContent);           //Создание локального менеджера с контентом главного UserControl для Логина и Регистрации

                localNavigationManager.Register(NavigationKeys.DataBaseViewKey);
                localNavigationManager.Navigate(NavigationKeys.DataBaseViewKey, array[2]);           //Переход к окну Базы данных
                this.localNavigationManager = localNavigationManager;                     //Необходимо хранить локальный навигатор
            }
            catch
            {

            }
        }
        //-----------------------------------------------------------------------------

        //Реализация интерфейса IMainUserControl
        public void MainUserControl()
        {

        }

        #endregion
    }
}
