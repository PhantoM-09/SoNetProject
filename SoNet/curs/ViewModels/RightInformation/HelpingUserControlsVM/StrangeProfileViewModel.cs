using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation.HelpingUserControlsVM
{
    class StrangeProfileViewModel : BaseViewModel, INavigationAware
    {
        #region Fields and Properties

        //Путь к стандартной картинке профиля
        private string standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");

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

        private string sex;
        public string Sex
        {
            get
            {
                return sex;
            }
            set
            {
                Set(ref sex, value);
            }
        }

        private string birthDay;
        public string BirthDay
        {
            get
            {
                return birthDay;
            }
            set
            {
                Set(ref birthDay, value);
            }
        }

        private string country;
        public string Country
        {
            get
            {
                return country;
            }
            set
            {
                Set(ref country, value);
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

        //Количество друзей
        private int friendCount;
        public int FriendCount
        {
            get
            {
                return friendCount;
            }
            set
            {
                Set(ref friendCount, value);
            }

        }

        //Количество подписчиков
        private int subscribeCount;
        public int SubscribeCount
        {
            get
            {
                return subscribeCount;
            }
            set
            {
                Set(ref subscribeCount, value);
            }

        }

        //Коллекция постов
        private ObservableCollection<Post> postsCollection;
        public ObservableCollection<Post> Posts
        {
            get
            {
                return postsCollection;
            }
            set
            {
                Set(ref postsCollection, value);
            }
        }


        private string addOrDelete;
        public string AddOrDelete
        {
            get
            {
                return addOrDelete;
            }
            set
            {
                Set(ref addOrDelete, value);
            }

        }

        //email того, кто зашел на страницу
        private string emailCurrentUser;

        //email того, кто владеет страницей
        private string emailProfileOwner;

        private ProfileImageManager imageManager;

        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public StrangeProfileViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            imageManager = new ProfileImageManager();
            CommentPost = new DelegateCommand(ExecuteCommentPost, CanCommentPost);
            GoBeFriends = new DelegateCommand(ExecuteGoBeFriends, CanGoBeFriends);
            FriendCount = 0;
            SubscribeCount = 0;
            GoToFiles = new DelegateCommand(ExecuteGoToFiles, CanGoToFiles);

        }
        #endregion


        #region Methods

        //Command для кнопки Комментирования поста-------------------------------------
        public ICommand CommentPost { get; }

        private void ExecuteCommentPost(object obj)
        {
            navigationManager.Register(NavigationKeys.CommentPostViewKey);
            navigationManager.Navigate(NavigationKeys.CommentPostViewKey, new object[] { emailCurrentUser, emailProfileOwner, obj });
        }
        private bool CanCommentPost(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Command для кнопки Добавить-----------------------------------------------------
        public ICommand GoBeFriends{ get; }

        private void ExecuteGoBeFriends(object obj)
        {
            try
            {
                using (UnitOfWork unit = new UnitOfWork())
                {
                    if(AddOrDelete == "Добавить")
                    {
                        //Добавление записи в БД о том, что он мой друг
                        Friend myFriend = new Friend() { UserEmail = emailCurrentUser, FriendEmail = emailProfileOwner, Status = 2 };
                        unit.FriendRepos.AddItem(myFriend);

                        //Добавление записи в БД о том, что я его подписчик
                        Friend IhisFriend = new Friend() { UserEmail = emailProfileOwner, FriendEmail = emailCurrentUser, Status = 1 };
                        unit.FriendRepos.AddItem(IhisFriend);

                        AddOrDelete = "Удалить";

                        SubscribeCount++;
                    }
                    else if(AddOrDelete == "Удалить")
                    {
                        
                        Friend iFriend = unit.FriendRepos.GetItem(new object[] { emailProfileOwner, emailCurrentUser });

                        //Если я подписчик удаляемого мною друга, значит необходимо удалить записи из БД о связи
                        if(iFriend.Status == 1)
                        {
                            unit.FriendRepos.DeleteItem(new object[] { emailCurrentUser, emailProfileOwner });
                            unit.FriendRepos.DeleteItem(new object[] { emailProfileOwner, emailCurrentUser });
                            AddOrDelete = "Добавить";
                            SubscribeCount--;
                        }
                        //Если я друг удаляемого мною друга, то надо установить, что он мой подписчик
                        else
                        {
                            Friend myFriend = unit.FriendRepos.GetItem(new object[] { emailCurrentUser, emailProfileOwner });
                            myFriend.Status = 1;
                            unit.FriendRepos.UpdateItem(myFriend);
                            AddOrDelete = "Принять";
                        }
                       
                    }
                    else
                    {
                        Friend myFriend = unit.FriendRepos.GetItem(new object[] { emailCurrentUser, emailProfileOwner });
                        myFriend.Status = 2;
                        unit.FriendRepos.UpdateItem(myFriend);
                        AddOrDelete = "Удалить";
                    }
                    unit.Save();

                }
                
            }
            catch
            {

            }
           
          
        }
        private bool CanGoBeFriends(object obj)
        {
            return true;
        }

        //--------------------------------------------------------


        //Command для кнопки Файлы-----------------------------------------------------
        public ICommand GoToFiles { get; }

        private void ExecuteGoToFiles(object obj)
        {
            navigationManager.Register(NavigationKeys.StrangeFilesViewKey);
            navigationManager.Navigate(NavigationKeys.StrangeFilesViewKey, new object[] { emailCurrentUser, emailProfileOwner });         //Переход к user control Файлы
        }
        private bool CanGoToFiles(object obj)
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
            try
            {
                var array = obj as object[];
                if (array == null)
                    return;

                emailCurrentUser = array[0] as string;

                var email = array[1] as string;
                if (email == null)
                    return;
                emailProfileOwner = email;
                
               

                imageManager.RegisterFileStorage(email);
                using (UnitOfWork unit = new UnitOfWork())
                {
                    IEnumerable<UserData> users = unit.UserDataRepos.GetItems();
                    foreach (var user in users)
                    {
                        //Подгрузка информации о пользователе, к которому зашли на страницу
                        if (user.Email == email)
                        {
                            LastName = user.LastName;
                            Name = user.Name;
                            Sex = user.Sex;
                            BirthDay = user.BirthDay;
                            Country = user.Country;

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

                            
                            var myFriend = unit.FriendRepos.GetItem(new object[] { emailCurrentUser, emailProfileOwner });

                            //Если связи нет, значит на кнопке слово "Добавить". Если связи есть, необходимо понять кто он мне: либо подписчик(1) тогда на кнопке слово "Принять", либо друг(2) и на кнопке слово "Удалить"
                            if (myFriend == null)
                            {
                                AddOrDelete = "Добавить";
                            }
                            else
                            {
                                if (myFriend.Status == 1)
                                {
                                    AddOrDelete = "Принять";
                                }
                                else
                                {
                                    AddOrDelete = "Удалить";


                                }
                            }
                            //Получаем всех людей у которых есть связь(подписчик, друг) с текущим пользователем
                            var my = unit.FriendRepos.GetItems().Where(f => f.UserEmail == user.Email).Select(t => t);
                            foreach (var link in my)
                            {
                                if (link.Status == 2)
                                {
                                    FriendCount++;
                                }
                                else
                                {
                                    SubscribeCount++;
                                }
                            }
                        }
                        
                    }

                    Posts = new ObservableCollection<Post>();
                    IEnumerable<Post> posts = unit.PostRepos.GetItems().Where(p => p.UserEmail == email).Select(t => t);
                    foreach (var post in posts)
                    {
                        if (post != null)
                            Posts.Add(post);
                    }
                }
            }
            catch
            {

            }
           
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
