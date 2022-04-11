using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.View.MainElements;
using curs.View.RightInformationView;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace curs.ViewModels.RightInformation
{
    public class MessageViewModel : BaseViewModel, INavigationAware, IMainUserControl
    {
        #region Fields and Properties
        private string standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");
        private int flagSearchFriends = 0;
        //Контакты
        private ObservableCollection<Friend> contacts;
        public ObservableCollection<Friend> Contacts       
        {
            get
            {
                return contacts;
            }
            set
            {
                
                Set(ref contacts, value);
                
            }
        }

        //Переменная для поискового запроса по котактам
        private ObservableCollection<Friend> tempFriendCollection;
        private string searchRequestFriend;
        public string SearchRequestFriend
        {
            get
            {
                return searchRequestFriend;
            }
            set
            {
                Set(ref searchRequestFriend, value);
            }

        }

        private Friend myFriend;
        public Friend MyFriend
        {
            get
            {
                return myFriend;
            }
            set
            {
                Set(ref myFriend, value);
                if (myFriend != null)
                {
                    OpenMessages.Execute(myFriend);
                }
            }
        }

        private ProfileImageManager imageManager;
        private string emailCurrentUser;
        private INavigationManager navigationManager;
        private INavigationManager localNavigationManager;
        #endregion

        #region Ctors
        public MessageViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;

            imageManager = new ProfileImageManager();
            Contacts = new ObservableCollection<Friend>();
            GoSearchFriends = new DelegateCommand(ExecuteGoSearchFriends, CanGoSearchFriends);
            GoCancelSearchFriends = new DelegateCommand(ExecuteGoCancelSearchFriends, CanGoCancelSearchFriends);
            OpenMessages = new DelegateCommand(ExecuteOpenMessages, CanOpenMessages);
        }


        #endregion


        #region Methods

        //Command для поиска из списка друзей, также здесь и отмена этого поиска------------------------------------
        public ICommand GoSearchFriends { get; }

        private void ExecuteGoSearchFriends(object obj)
        {
            if (flagSearchFriends == 0)
                tempFriendCollection = Contacts;

            Contacts = new ObservableCollection<Friend>(tempFriendCollection.Where(f => f.FriendInfo.LastName == SearchRequestFriend).Select(t => t));
            flagSearchFriends = 1;
        }
        private bool CanGoSearchFriends(object obj)
        {
            return true;
        }

        public ICommand GoCancelSearchFriends { get; }

        private void ExecuteGoCancelSearchFriends(object obj)
        {
            Contacts = tempFriendCollection;
            SearchRequestFriend = "";
            flagSearchFriends = 0;
        }
        private bool CanGoCancelSearchFriends(object obj)
        {
            if (flagSearchFriends == 0)
                return false;
            else
                return true;
        }

        //----------------------------------------------------------

        //Command для элемента listbox, открытие чужого профиля------------------------------------
        public ICommand OpenMessages { get; }
        private void ExecuteOpenMessages(object obj)
        {
            localNavigationManager.Register(NavigationKeys.SendMessageViewKey);
            localNavigationManager.Navigate(NavigationKeys.SendMessageViewKey, new object[] {emailCurrentUser, myFriend.FriendEmail });

        }
        private bool CanOpenMessages(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Реализация интерфейса INavigationAware--------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            //---------------------------------------
            object[] array = obj as object[];
            if (array == null)
                return;
            var dispatcher = array[0] as Dispatcher;
            var view = array[1] as MessageView;

            NavigationManager localNavigationManager = new NavigationManager(dispatcher, view.SendMessageContent);

            this.localNavigationManager = localNavigationManager;
            //---------------------------------------

            emailCurrentUser = array[2] as string;

            using (UnitOfWork unit = new UnitOfWork())
            {
                Contacts = new ObservableCollection<Friend>(unit.FriendRepos.GetItems().Where(f => f.UserEmail == emailCurrentUser).Select(t => t));

                for (int i = 0; i < Contacts.Count; i++)
                {

                    if (Contacts[i].FriendInfo.ImageId == null)
                        Contacts[i].FriendInfo.ImageName = standardProfileImageName;
                    else
                    {
                        imageManager = new ProfileImageManager();
                        imageManager.RegisterFileStorage(Contacts[i].FriendInfo.Email);
                        imageManager.ManagementPC(Contacts[i].FriendInfo.ImageId, Contacts[i].FriendInfo.ImageName);
                        if (!File.Exists(Contacts[i].FriendInfo.ImageName))
                            Contacts[i].FriendInfo.ImageName = standardProfileImageName;
                    }
                }
            }

        }

        //----------------------------------------------------------------------

        //Интерфейс IMainUserControl. Здесь он для необходим, так как данное окно имеет contentcontrol--------------------------------------------
        public void MainUserControl()
        {

        }
        //----------------------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
