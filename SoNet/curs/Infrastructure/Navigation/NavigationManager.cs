using curs.ServiceChat;
using curs.View.LoginRegistrationView;
using curs.View.MainElements;
using curs.View.RightInformationView;
using curs.View.RightInformationView.Admin;
using curs.View.RightInformationView.HelpingUserControls;
using curs.View.SendMessage;
using curs.View.Settings;
using curs.ViewModels.LoginRegistration;
using curs.ViewModels.RightInformation;
using curs.ViewModels.RightInformation.HelpingUserControlsVM;
using DatabaseClasses.UnitOfWorkPattern;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace curs.Infrastructure.Navigation
{


    public class NavigationManager : INavigationManager, IServiceChatCallback
    {
        #region Fields
        private string emailCurrentUser;
        public string EmailCurrentUser
        {
            get
            {
                return emailCurrentUser;
            }
            set
            {
                emailCurrentUser = value;
            }
        }
        event Action<string> DisconnectUser;

        Dispatcher dispatcher;
        ContentControl userElementsContentControl;                                             //ContentControl для вставки в его свойство Content определенного View
        Dictionary<string, object> _viewModelsByKeys = new Dictionary<string, object>();       //Словарь: [переданный ключ соотв. View] = объект соотв. ViewModel
        Dictionary<Type, object> viewObjectByViewModelType = new Dictionary<Type, object>();     //Словарь: [тип ViewModel] = объект соотв. View
        #endregion

        #region Ctors
        public NavigationManager(Dispatcher dispatcher, ContentControl userElementsContentControl)
        {
            this.dispatcher = dispatcher;
            this.userElementsContentControl = userElementsContentControl;
            DisconnectUser += DisconnectUserFromChat;
        }
        #endregion

        #region Methods
        //Навигация: последнее желание закрывающегося View, определение следующего VM по переданному ключу,
        //Первое желание до создания View, создание View, "открытие" View
        public void Navigate(string navigationKey, object obj = null)
        {
            
            if(navigationKey == null)
                throw new ArgumentNullException("navigationKey");

            if (obj != null)
                EmailCurrentUser = obj as string;

            ExecutionActionInMainThread(() =>
            {
                DisconnectUser.Invoke(EmailCurrentUser);
                DoSomethingBeforeClose();                                           //Последнее желание закрывающегося View
                var viewModel = GetViewModelFromDictionary(navigationKey);          //Определение объекта VM по заданном ключу
                DoSomethingBeforeOpen(viewModel, obj);                                   //Предварительные действия открывающегося View

                var view = CreateViewByViewModel(viewModel);                        //Определение объекта View по заданному ViewModel
                userElementsContentControl.Content = view;                          //Псевдооткрытие View
            });

        }

        //Заполнение словарей
        private void AddElementToDictionary<TViewModel, TView>(TViewModel viewModel, TView view, string navigationKey)
            where TViewModel : class
            where TView : FrameworkElement
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            if (view == null)
                throw new ArgumentNullException("view");
            if (navigationKey == null)
                throw new ArgumentNullException("navigationKey");

            _viewModelsByKeys[navigationKey] = viewModel;                  //Заполнение словаря объектами ViewModel по переданному ключу
            viewObjectByViewModelType[typeof(TViewModel)] = view;            //Заполнение словаря объектами View по ключу ввиде типа соответствующей ViewModel
        }

        //Выполнение переданного действия в главном потоке
        private void ExecutionActionInMainThread(Action action) => dispatcher.Invoke(action);

        //Взятие объекта ViewModel из словаря по переданному ключу
        private object GetViewModelFromDictionary(string navigationKey)
        {
            if (navigationKey == null)
                throw new ArgumentNullException("navigationKey");
            return _viewModelsByKeys[navigationKey];
        }

        //Взятие объекта View на базе переданного ViewModel и установление контекста взятого View
        private FrameworkElement CreateViewByViewModel(object viewModel)
        {
            var view = (FrameworkElement)viewObjectByViewModelType[viewModel.GetType()];;
            view.DataContext = viewModel;
            return view;
        }

        private void DoSomethingBeforeClose(object obj = null)                   //Навигатор предлагает сделать свое последнее желание закрывающемуся View
        {
            var oldView = userElementsContentControl.Content as FrameworkElement;
            if (oldView == null)
                return;

            var viewModelOldView = oldView.DataContext as INavigationAware;   //Взятие ViewModel закрывающегося View

            //Это условие выполниться только для главного View, т к он не реализует INavigationAware
            if (viewModelOldView == null)
                return;

            viewModelOldView.WantDoSomethingBeforeClose();
        }

        private void DoSomethingBeforeOpen(object viewModel, object obj = null)                   //Навигатор предлагает сделать свое первое желание открывающемуся View
        {
            var transformedViewModel = viewModel as INavigationAware;   //Приведение объекта ViewModel к интерфейсу для последующего вызова метода приведенного интерфейса

            var mainVM = transformedViewModel as IMainUserControl;                 
            if (mainVM == null)
            {
                transformedViewModel.WantDoSomethingBeforeOpen(obj);
                return;
            }

            object[] objectArray = new object[] { dispatcher, CreateViewByViewModel(mainVM), obj };          //Массив состоящий из диспетчера и обекта View соотв. ViewModel
            transformedViewModel.WantDoSomethingBeforeOpen(objectArray);
        }



        public void Register(string navigationKey, INavigationManager navigationManager = null)
        {
            if (navigationKey == null)
                throw new ArgumentNullException("navigationKey");

            switch(navigationKey)
            {
                //Главные User Controls
                case NavigationKeys.ContextLoginRegisterKey:
                    AddElementToDictionary<ContextLogRegViewModel, ContentForLoginRegistration>(new ContextLogRegViewModel(this), new ContentForLoginRegistration(), navigationKey);
                    break;
                case NavigationKeys.ContextRightInfoViewKey:
                    AddElementToDictionary<ContextRightInfoViewModel, ContentForRightInfo>(new ContextRightInfoViewModel(this), new ContentForRightInfo(), navigationKey);
                    break;
                case NavigationKeys.AdminContextRightInfoViewKey:
                    AddElementToDictionary<AdminContextRightInfoViewModel, AdminContentForRightInfo>(new AdminContextRightInfoViewModel(this), new AdminContentForRightInfo(), navigationKey);
                    break;

                //Страницы Логина и Регистрации
                case NavigationKeys.LoginViewKey:
                    AddElementToDictionary<LoginViewModel, LoginView>(new LoginViewModel(this, navigationManager ?? throw new Exception("отсутствие навигатора для Login")), new LoginView(), navigationKey);
                    break;
                case NavigationKeys.RegisterViewKey:
                    AddElementToDictionary<RegisterViewModel, RegistrationView>(new RegisterViewModel(this), new RegistrationView(), navigationKey);
                    break;
                case NavigationKeys.RegisterEndViewKey:
                    AddElementToDictionary<RegisterEndViewModel, RegistrationEndView>(new RegisterEndViewModel(this), new RegistrationEndView(), navigationKey);
                    break;
                case NavigationKeys.RegisterPictureViewKey:
                    AddElementToDictionary<RegistrationPictureViewModel, RegistrationAddPicture>(new RegistrationPictureViewModel(this, navigationManager ?? throw new Exception("отсутствие навигатора для RegisterPic")),
                                                                                                                         new RegistrationAddPicture(), navigationKey);
                    break;
                case NavigationKeys.ErrorMessageViewKey:
                    AddElementToDictionary<ErrorMessageViewModel, ErrorMessage>(new ErrorMessageViewModel(this), new ErrorMessage(), navigationKey);
                    break;

                //Основные страницы приложения
                case NavigationKeys.ProfileViewKey:
                    AddElementToDictionary<ProfileViewModel, ProfileView>(new ProfileViewModel(this), new ProfileView(), navigationKey);
                    break;
                case NavigationKeys.FriendsViewKey:
                    AddElementToDictionary<FriendsViewModel, FriendsView>(new FriendsViewModel(this), new FriendsView(), navigationKey);
                    break;
                case NavigationKeys.MessageViewKey:
                    AddElementToDictionary<MessageViewModel, MessageView>(new MessageViewModel(this), new MessageView(), navigationKey);
                    break;
                case NavigationKeys.MyFilesViewKey:
                    AddElementToDictionary<MyFilesViewModel, MyFilesView>(new MyFilesViewModel(this), new MyFilesView(), navigationKey);
                    break;
                case NavigationKeys.SettingsViewKey :
                    AddElementToDictionary<SettingsViewModel, SettingsView>(new SettingsViewModel(this), new SettingsView(), navigationKey);
                    break;
                case NavigationKeys.AddPostViewKey:
                    AddElementToDictionary<AddPostViewModel, AddPostView>(new AddPostViewModel(this), new AddPostView(), navigationKey);
                    break;
                case NavigationKeys.AddChangePostViewKey:
                    AddElementToDictionary<AddChangePostViewModel, AddChangePostView>(new AddChangePostViewModel(this), new AddChangePostView(), navigationKey);
                    break;
                case NavigationKeys.ExitViewKey:
                    AddElementToDictionary<ExitViewModel, ExitView>(new ExitViewModel(this, (NavigationManager)(navigationManager ?? throw new Exception("отсутствие навигатора для Exit"))), new ExitView(), navigationKey);
                    break;
                case NavigationKeys.DeletePostViewKey:
                    AddElementToDictionary<DeletePostViewModel, DeletePostVIew>(new DeletePostViewModel(this), new DeletePostVIew(), navigationKey);
                    break;
                case NavigationKeys.CommentPostViewKey:
                    AddElementToDictionary<CommentPostViewModel, CommentPostView>(new CommentPostViewModel(this), new CommentPostView(), navigationKey);
                    break;
                case NavigationKeys.SearchResultsViewKey:
                    AddElementToDictionary<SearchResultsViewModel, SearchResultsView>(new SearchResultsViewModel(this), new SearchResultsView(), navigationKey);
                    break;
                case NavigationKeys.StrangeFilesViewKey:
                    AddElementToDictionary<StrangeFilesViewModel, StrangeFilesView>(new StrangeFilesViewModel(this), new StrangeFilesView(), navigationKey);
                    break;
                case NavigationKeys.StrangeProfileViewKey:
                    AddElementToDictionary<StrangeProfileViewModel, StrangeProfileView>(new StrangeProfileViewModel(this), new StrangeProfileView(), navigationKey);
                    break;
                case NavigationKeys.SendMessageViewKey:
                    AddElementToDictionary<SendMessageViewModel, SendMessageView>(new SendMessageViewModel(this), new SendMessageView(), navigationKey);
                    break;
                case NavigationKeys.DataBaseViewKey:
                    AddElementToDictionary<DataBaseViewModel, DataBaseView>(new DataBaseViewModel(this), new DataBaseView(), navigationKey);
                    break;
            }
        }

        
        public string AddImage()
        {
            OpenFileDialog choiceWindow = new OpenFileDialog();

            choiceWindow.Filter = "Файлы рисунков (*.jpg)|*.jpg";
            choiceWindow.Multiselect = false;
            if (choiceWindow.ShowDialog() ==true)
            {
                return choiceWindow.FileName;
            }
            return "";
        }
        //------------------------------------------------------

        public string AddFile()
        {
            OpenFileDialog choiceWindow = new OpenFileDialog();

            choiceWindow.Filter = "Файлы рисунков (*.jpg)|*.jpg|Файлы видео(*.mp4)|*.mp4|Файлы музыки(*.mp3)|*.mp3";
            choiceWindow.FilterIndex = 1;
            choiceWindow.Multiselect = false;
            if (choiceWindow.ShowDialog() == true)
            {
                return choiceWindow.FileName;
            }
            return "";
        }

        public string DownloadFile()
        {
            SaveFileDialog choiceWindow = new SaveFileDialog();

            choiceWindow.Filter = "Файлы рисунков (*.jpg)|*.jpg|Файлы видео(*.mp4)|*.mp4|Файлы музыки(*.mp3)|*.mp3";
            choiceWindow.FilterIndex = 1;
            if (choiceWindow.ShowDialog() == true)
            {
                return choiceWindow.FileName;
            }
            return "";
        }

        private void DisconnectUserFromChat(string emailDisconnetcUser)
        {
            try
            {
                using (UnitOfWork unit = new UnitOfWork())
                {

                    var friends = unit.FriendRepos.GetItems().Where(f => f.UserEmail == emailDisconnetcUser).Select(u => u);

                    ServiceChatClient client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                    foreach (var friend in friends)
                    {
                        client.Disconnect(emailDisconnetcUser, friend.FriendEmail);
                    }

                }

            }
            catch
            {

            }
            
        }

        public void MessageCallBack(string message, string idUser)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
