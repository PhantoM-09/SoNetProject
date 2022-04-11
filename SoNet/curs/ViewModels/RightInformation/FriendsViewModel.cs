using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
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
    public class FriendsViewModel : BaseViewModel, INavigationAware, IDataErrorInfo
    {
        #region Fields and Properties
        //Путь к стандартной картинке профиля
        private int flagSearchFriends = 0;
        private int flagSearchSubscribe = 0;
        private string standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");

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
                    OpenStrangeProfile.Execute(myFriend);
                }
            }
        }

        //Коллекция друзей
        private ObservableCollection<Friend> tempFriendCollection;
        private ObservableCollection<Friend> friendCollection;
        public ObservableCollection<Friend> FriendCollection
        {
            get
            {
                return friendCollection;
            }
            set
            {
                Set(ref friendCollection, value);
            }
        }

        //Коллекция подписчиков
        private ObservableCollection<Friend> tempSubscribeCollection;
        private ObservableCollection<Friend> subscribeCollection;
        public ObservableCollection<Friend> SubscribeCollection
        {
            get
            {
                return subscribeCollection;
            }
            set
            {
                Set(ref subscribeCollection, value);
            }
        }

        //Переменная для поискового запроса по друзьям
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

        //Переменная для поискового запроса по подписчикам
        private string searchRequestSubscribe;
        public string SearchRequestSubscribe
        {
            get
            {
                return searchRequestSubscribe;
            }
            set
            {
                Set(ref searchRequestSubscribe, value);
            }

        }

        private string emailCurrentUser;
        Dictionary<string, string> ValidationErrors;
        private ProfileImageManager imageManager;
        private INavigationManager navigationManager;
        #endregion


        #region Ctors
        public FriendsViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            ValidationErrors = new Dictionary<string, string>();
            imageManager = new ProfileImageManager();

            FriendCollection = new ObservableCollection<Friend>();
            SubscribeCollection = new ObservableCollection<Friend>();

            OpenStrangeProfile = new DelegateCommand(ExecuteOpenStrangeProfile, CanOpenStrangeProfile);
            GoSearchFriends = new DelegateCommand(ExecuteGoSearchFriends, CanGoSearchFriends);
            GoCancelSearchFriends = new DelegateCommand(ExecuteGoCancelSearchFriends, CanGoCancelSearchFriends);
            GoSearchSubscribers = new DelegateCommand(ExecuteGoSearchSubscribers, CanGoSearchSubscribers);
            GoCancelSearchSubscribers = new DelegateCommand(ExecuteGoCancelSearchSubscribers, CanGoCancelSearchSubscribers);
            GoBeFriend = new DelegateCommand(ExecuteGoBeFriend, CanGoBeFriend);
        }
        #endregion


        #region Methods

        //Command для элемента listbox, открытие чужого профиля------------------------------------
        public ICommand OpenStrangeProfile{ get; }
        private void ExecuteOpenStrangeProfile(object obj)
        {
            navigationManager.Register(NavigationKeys.StrangeProfileViewKey);
            navigationManager.Navigate(NavigationKeys.StrangeProfileViewKey, new object[] { MyFriend.UserEmail, MyFriend.FriendEmail } );
        }
        private bool CanOpenStrangeProfile(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Command для поиска из списка друзей, также здесь и отмена этого поиска------------------------------------
        public ICommand GoSearchFriends{ get; }

        private void ExecuteGoSearchFriends(object obj)
        {
            if(flagSearchFriends == 0)
                tempFriendCollection = FriendCollection;

            FriendCollection = new ObservableCollection<Friend>(tempFriendCollection.Where(f => f.FriendInfo.LastName == SearchRequestFriend).Select(t => t));
            flagSearchFriends = 1;
        }
        private bool CanGoSearchFriends(object obj)
        {
            return true;
        }
        //----------------------------------
        public ICommand GoCancelSearchFriends { get; }

        private void ExecuteGoCancelSearchFriends(object obj)
        {
            FriendCollection = tempFriendCollection;
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


        //Command для поиска из списка подписчиков, также здесь и отмена этого яяпоиска------------------------------------
        public ICommand GoSearchSubscribers { get; }

        private void ExecuteGoSearchSubscribers(object obj)
        {
            if (flagSearchSubscribe == 0)
                tempSubscribeCollection = SubscribeCollection;

            SubscribeCollection = new ObservableCollection<Friend>(tempSubscribeCollection.Where(f => f.FriendInfo.LastName == SearchRequestSubscribe).Select(t => t));
            flagSearchSubscribe = 1;
        }
        private bool CanGoSearchSubscribers(object obj)
        {
            return true;
        }

        public ICommand GoCancelSearchSubscribers { get; }

        private void ExecuteGoCancelSearchSubscribers(object obj)
        {
            SubscribeCollection = tempSubscribeCollection;
            SearchRequestSubscribe = "";
            flagSearchSubscribe = 0;
        }
        private bool CanGoCancelSearchSubscribers(object obj)
        {
            if (flagSearchSubscribe == 0)
                return false;
            else
                return true;
        }

        //----------------------------------------------------------

        //Command для элемента listbox, открытие чужого профиля------------------------------------
        public ICommand GoBeFriend{ get; }
        private void ExecuteGoBeFriend(object obj)
        {
            Friend futureLink = obj as Friend;
            if (futureLink == null)
                return;

            using(UnitOfWork unit = new UnitOfWork())
            {
                Friend myFriend = unit.FriendRepos.GetItem(new object[] { futureLink.UserEmail, futureLink.FriendEmail });
                myFriend.Status = 2;
                unit.FriendRepos.UpdateItem(myFriend);
                unit.Save();

                FriendCollection.Add(futureLink);
                SubscribeCollection.Remove(futureLink);
                
            } 
        }
        private bool CanGoBeFriend(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Интерфейс IDataErrorInfo------------------------------------------------------------------------------------------------
        public string Error => throw new NotImplementedException();

        public string this[string columnName] => ValidationErrors.ContainsKey(columnName) ? ValidationErrors[columnName] : null;
        //------------------------------------------------------------------------------------------------------------------------

        //Проверка валидации формы
        public bool IsValid() => ValidationErrors.Values.All(x => x == null);

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
            using(UnitOfWork unit = new UnitOfWork())
            {
                FriendCollection = new ObservableCollection<Friend>(unit.FriendRepos.GetItems().Where(f => f.UserEmail == email && f.Status == 2).Select(t => t));


                for (int i = 0; i < FriendCollection.Count; i++)
                {

                    if (FriendCollection[i].FriendInfo.ImageId == null)
                        FriendCollection[i].FriendInfo.ImageName = standardProfileImageName;
                    else
                    {
                        imageManager = new ProfileImageManager();
                        imageManager.RegisterFileStorage(FriendCollection[i].FriendInfo.Email);
                        imageManager.ManagementPC(FriendCollection[i].FriendInfo.ImageId, FriendCollection[i].FriendInfo.ImageName);
                        if (!File.Exists(FriendCollection[i].FriendInfo.ImageName))
                            FriendCollection[i].FriendInfo.ImageName = standardProfileImageName;
                    }
                }

                SubscribeCollection = new ObservableCollection<Friend>(unit.FriendRepos.GetItems().Where(f => f.UserEmail == email && f.Status == 1).Select(t => t));

                for (int i = 0; i < SubscribeCollection.Count; i++)
                {

                    if (SubscribeCollection[i].FriendInfo.ImageId == null)
                        SubscribeCollection[i].FriendInfo.ImageName = standardProfileImageName;
                    else
                    {
                        imageManager = new ProfileImageManager();
                        imageManager.RegisterFileStorage(SubscribeCollection[i].FriendInfo.Email);
                        imageManager.ManagementPC(SubscribeCollection[i].FriendInfo.ImageId, SubscribeCollection[i].FriendInfo.ImageName);
                        if (!File.Exists(SubscribeCollection[i].FriendInfo.ImageName))
                            SubscribeCollection[i].FriendInfo.ImageName = standardProfileImageName;
                    }
                }
            }
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
