using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ServiceChat;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation
{
    class SendMessageViewModel : BaseViewModel, INavigationAware, IDataErrorInfo, IServiceChatCallback
    {
        #region Fields and Properties
        private int flagMessage = 0;


        private string lastName;
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                Set(ref lastName, value);
            }

        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                Set(ref name, value);
            }

        }

        private string imageSource;
        public string ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                Set(ref imageSource, value);
            }

        }

        private string messageText;
        public string MessageText
        {
            get
            {
                return messageText;
            }
            set
            {
                Set(ref messageText, value);

                flagMessage = 1;

                if (MessageText == "" || MessageText == " " || MessageText == null)
                    ValidationErrors["MessageText"] = "Неверный формат! Поле не может быть пустым";
                else
                    ValidationErrors["MessageText"] = null;
            }

        }

        //Коллекция сообщений
        private ObservableCollection<string> messages;
        public ObservableCollection<string> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                Set(ref messages, value);
            }

        }


        private string standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");
        private string emailCurrentUser;
        private string emailUserFriend;
        private ProfileImageManager imageManager;
        Dictionary<string, string> ValidationErrors;

        ServiceChatClient client;
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public SendMessageViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));

            imageManager = new ProfileImageManager();
            ValidationErrors = new Dictionary<string, string>();
            GoSendMessage = new DelegateCommand(ExecuteGoSendMessage, CanGoSendMessage);
            Messages = new ObservableCollection<string>();
        }
        #endregion


        #region Methods

        //Command для кнопки Отправить---------------------------------------------------
        public ICommand GoSendMessage { get; }

        private void ExecuteGoSendMessage(object obj)
        {
            try
            {
                client.SendMessageToChat(MessageText, emailCurrentUser, emailUserFriend);
                MessageText = "";
            }
            catch
            {

            }

        }
        private bool CanGoSendMessage(object obj)
        {
            if (flagMessage == 0)
                return false;
            return IsValid();
        }


        //------------------------------------------------------------------------------------
        //Реализация интерфейса INavigationAware--------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            try
            {
                var array = obj as object[];
                if (array == null)
                    return;

                emailCurrentUser = array[0] as string;
                emailUserFriend = array[1] as string;

                imageManager.RegisterFileStorage(emailUserFriend);
                using (UnitOfWork unit = new UnitOfWork())
                {
                    IEnumerable<UserData> users = unit.UserDataRepos.GetItems();
                    foreach (var user in users)
                    {
                        //Подгрузка информации о пользователе, к которому зашли на страницу
                        if (user.Email == emailUserFriend)
                        {
                            LastName = user.LastName;
                            Name = user.Name;

                            //Загрузка картинки профиля-----------------
                            if (user.ImageId == null)
                                ImageSource = standardProfileImageName;
                            else
                            {
                                imageManager.ManagementPC(user.ImageId, user.ImageName);
                                if (File.Exists(user.ImageName))
                                    ImageSource = user.ImageName;
                                else
                                    ImageSource = standardProfileImageName;
                            }
                            //-----------------------------------------
                        }
                    }
                }

                client.Connect(emailCurrentUser, emailUserFriend);

            }
            catch
            {

            }
            

        }
        //----------------------------------------------------------------------

        //Интерфейс IDataErrorInfo------------------------------------------------------------------------------------------------
        public string Error => throw new NotImplementedException();

        public string this[string columnName] => ValidationErrors.ContainsKey(columnName) ? ValidationErrors[columnName] : null;
        //------------------------------------------------------------------------------------------------------------------------

        //Проверка валидации формы
        public bool IsValid() => ValidationErrors.Values.All(x => x == null);

        public void MessageCallBack(string message, string idUser)
        {
            string result;
            using(UnitOfWork unit = new UnitOfWork())
            {
                var userSender = unit.UserDataRepos.GetItem(new object[] { idUser });
                result = DateTime.Now.ToShortTimeString() + " " + userSender.LastName + " " + userSender.Name + ": " + message;
            }

            Messages.Add(result);
        }
        #endregion
    }
}
