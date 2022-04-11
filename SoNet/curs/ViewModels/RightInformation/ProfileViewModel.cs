using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation
{
    class ProfileViewModel : BaseViewModel, INavigationAware
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


        private string emailCurrentUser;
        private ProfileImageManager imageManager;
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public ProfileViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            imageManager = new ProfileImageManager();
            Posts = new ObservableCollection<Post>();
            FriendCount = 0;
            SubscribeCount = 0;

            AddPost = new DelegateCommand(ExecuteAddPost, CanAddPost);
            AddChangePost = new DelegateCommand(ExecuteAddChangePost, CanAddChangePost);
            DeletePost = new DelegateCommand(ExecuteDeletePost, CanDeletePost);
            CommentPost = new DelegateCommand(ExecuteCommentPost, CanCommentPost);
            GoToMyFiles = new DelegateCommand(ExecuteGoToMyFiles, CanGoToMyFiles);
            GoToSettings = new DelegateCommand(ExecuteGoToSettings, CanGoToSettings);
            GoToFriends = new DelegateCommand(ExecuteGoToFriends, CanGoToFriends);
        }
        #endregion


        #region Methods

        //Command для кнопки Удаления поста-------------------------------------
        public ICommand DeletePost { get; }

        private void ExecuteDeletePost(object obj)
        {
            navigationManager.Register(NavigationKeys.DeletePostViewKey);
            navigationManager.Navigate(NavigationKeys.DeletePostViewKey, new object[] {emailCurrentUser, obj as Post });
        }
        private bool CanDeletePost(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Command для кнопки Добавления поста------------------------------------
        public ICommand AddPost { get; }

        private void ExecuteAddPost(object obj)
        {
            navigationManager.Register(NavigationKeys.AddPostViewKey);
            navigationManager.Navigate(NavigationKeys.AddPostViewKey, emailCurrentUser);
        }
        private bool CanAddPost(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Command для кнопки Изменение поста------------------------------------
        public ICommand AddChangePost { get; }

        private void ExecuteAddChangePost(object obj)
        {
            navigationManager.Register(NavigationKeys.AddChangePostViewKey);
            navigationManager.Navigate(NavigationKeys.AddChangePostViewKey, new object[] { emailCurrentUser, obj as Post });
        }
        private bool CanAddChangePost(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Command для кнопки Комментирования поста-------------------------------------
        public ICommand CommentPost { get; }

        private void ExecuteCommentPost(object obj)
        {
            navigationManager.Register(NavigationKeys.CommentPostViewKey);
            navigationManager.Navigate(NavigationKeys.CommentPostViewKey, new object[] { emailCurrentUser, emailCurrentUser, obj});
        }
        private bool CanCommentPost(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Command для кнопки Файлы-----------------------------------------------------
        public ICommand GoToMyFiles { get; }

        private void ExecuteGoToMyFiles(object obj)
        {
            navigationManager.Register(NavigationKeys.MyFilesViewKey);
            navigationManager.Navigate(NavigationKeys.MyFilesViewKey, emailCurrentUser);         //Переход к user control Файлы
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
            navigationManager.Register(NavigationKeys.SettingsViewKey);
            navigationManager.Navigate(NavigationKeys.SettingsViewKey, emailCurrentUser);         //Переход к user control Настройки
        }
        private bool CanGoToSettings(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        //Command для кнопки Друзья---------------------------------------------------
        public ICommand GoToFriends { get; }

        private void ExecuteGoToFriends(object obj)
        {
            navigationManager.Register(NavigationKeys.FriendsViewKey);
            navigationManager.Navigate(NavigationKeys.FriendsViewKey, emailCurrentUser);         //Переход к user control Друзья
        }
        private bool CanGoToFriends(object obj)
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

            UserData onlineUser = new UserData();
            FriendCount = 0;
            SubscribeCount = 0;
            emailCurrentUser = email;
            imageManager.RegisterFileStorage(email); 
            using (UnitOfWork unit = new UnitOfWork())
            {
                IEnumerable<UserData> users = unit.UserDataRepos.GetItems();
                foreach(var user in users)
                {
                    if(user.Email == email)
                    {
                        onlineUser = user;
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

                        //Получаем всех людей у которых есть связь(подписчик, друг) с текущим пользователем
                        var my = unit.FriendRepos.GetItems().Where(f => f.UserEmail == user.Email).Select(t => t);
                        foreach(var link in my)
                        {
                            if(link.Status == 2)
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

                //Пометка о том, что пользователь зашел в сеть
                onlineUser.IsOnline = true;
                unit.UserDataRepos.UpdateItem(onlineUser);
                Posts = new ObservableCollection<Post>();
                IEnumerable<Post> posts = unit.PostRepos.GetItems().Where(p => p.UserEmail == email).Select(t => t);
                foreach(var post in posts)
                {
                    if(post!=null)
                        Posts.Add(post);
                }

                unit.Save();
            }
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
