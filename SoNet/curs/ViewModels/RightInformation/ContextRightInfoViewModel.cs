using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.View.MainElements;
using curs.View.RightInformationView;
using curs.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace curs.ViewModels.RightInformation
{
    class ContextRightInfoViewModel : BaseViewModel, INavigationAware, IMainUserControl, IDataErrorInfo
    {
        #region Fields and Property

        //Переменная для поискового запроса
        private string searchRequest;
        public string SearchRequest
        {
            get
            {
                return searchRequest;
            }
            set
            {
                Set(ref searchRequest, value);
            }
        }

        private string emailCurrentUser;
        private readonly INavigationManager navigationManager;
        private INavigationManager localNavigationManager;
        Dictionary<string, string> ValidationErrors;
        #endregion


        #region Ctors

        public ContextRightInfoViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            ValidationErrors = new Dictionary<string, string>();

            GoToProfile = new DelegateCommand(ExecuteGoToProfile, CanGoToProfile);
            GoToFriends = new DelegateCommand(ExecuteGoToFriends, CanGoToFriends);
            GoToMessage = new DelegateCommand(ExecuteGoToMessage, CanGoToMessage);
            GoToMyFiles = new DelegateCommand(ExecuteGoToMyFiles, CanGoToMyFiles);
            GoToSettings = new DelegateCommand(ExecuteGoToSettings, CanGoToSettings);
            GoToExit = new DelegateCommand(ExecuteGoToExit, CanGoToExit);
            GoToSearchResults = new DelegateCommand(ExecuteGoToSearchResults, CanGoToSearchResults);
        }

        #endregion


        #region Methods

        //Command для кнопки Моя страница---------------------------------------------------
        public ICommand GoToProfile { get; }

        private void ExecuteGoToProfile(object obj)
        {
            localNavigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);         //Переход к user control Профиль
        }
        private bool CanGoToProfile(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        //Command для кнопки Друзья---------------------------------------------------
        public ICommand GoToFriends { get; }

        private void ExecuteGoToFriends(object obj)
        {
            localNavigationManager.Register(NavigationKeys.FriendsViewKey);
            localNavigationManager.Navigate(NavigationKeys.FriendsViewKey, emailCurrentUser);         //Переход к user control Друзья
        }
        private bool CanGoToFriends(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        //Command для кнопки Сообщения---------------------------------------------------
        public ICommand GoToMessage { get; }

        private void ExecuteGoToMessage(object obj)
        {
            localNavigationManager.Register(NavigationKeys.MessageViewKey);
            localNavigationManager.Navigate(NavigationKeys.MessageViewKey, emailCurrentUser);         //Переход к user control Сообщения
        }
        private bool CanGoToMessage(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        //Command для кнопки Файлы-----------------------------------------------------
        public ICommand GoToMyFiles { get; }

        private void ExecuteGoToMyFiles(object obj)
        {
            localNavigationManager.Register(NavigationKeys.MyFilesViewKey);
            localNavigationManager.Navigate(NavigationKeys.MyFilesViewKey, emailCurrentUser);         //Переход к user control Файлы
        }
        private bool CanGoToMyFiles(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        //Command для кнопки Настройки(значок шестеренки)---------------------------------------------------
        public ICommand GoToSettings { get; }

        private void ExecuteGoToSettings(object obj)
        {
            localNavigationManager.Register(NavigationKeys.SettingsViewKey);
            localNavigationManager.Navigate(NavigationKeys.SettingsViewKey, emailCurrentUser);         //Переход к user control Настройки
        }
        private bool CanGoToSettings(object obj)
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

        //Command для кнопки Поиск-----------------------------------
        public ICommand GoToSearchResults { get; }

        private void ExecuteGoToSearchResults(object obj)
        {
            localNavigationManager.Register(NavigationKeys.SearchResultsViewKey);
            localNavigationManager.Navigate(NavigationKeys.SearchResultsViewKey, new object[] { emailCurrentUser, SearchRequest });             //Переход к user control Результаты поиска
            SearchRequest = "";
        }
        private bool CanGoToSearchResults(object obj)
        {
            return true;
        }
        //-----------------------------------------------------------

        //Интерфейс IDataErrorInfo------------------------------------------------------------------------------------------------
        public string Error => throw new NotImplementedException();

        public string this[string columnName] => ValidationErrors.ContainsKey(columnName) ? ValidationErrors[columnName] : null;
        //------------------------------------------------------------------------------------------------------------------------

        //Проверка валидации формы
        public bool IsValid() => ValidationErrors.Values.All(x => x == null);

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
                var view = array[1] as ContentForRightInfo;
                emailCurrentUser = array[2] as string;

                NavigationManager localNavigationManager = new NavigationManager(dispatcher, view.RightInformationContent);           //Создание локального менеджера с контентом главного UserControl для Логина и Регистрации

                //Регистрация user controls который будут вставляться в контент главного UserControl для Правой информации-------------------------------------------------------------------------------------------------------------------------
                localNavigationManager.Register(NavigationKeys.ProfileViewKey);
               
                

                localNavigationManager.Register(NavigationKeys.StrangeFilesViewKey);
               
                //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                localNavigationManager.Navigate(NavigationKeys.ProfileViewKey, array[2]);           //Переход к окну Профиля
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
