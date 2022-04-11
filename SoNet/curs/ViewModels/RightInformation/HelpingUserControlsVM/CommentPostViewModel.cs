using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Models;
using DatabaseClasses.UnitOfWorkPattern;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FileManagement;

namespace curs.ViewModels.RightInformation.HelpingUserControlsVM
{
    class CommentPostViewModel : BaseViewModel, INavigationAware, IDataErrorInfo
    {
        #region Fields and Properties
        private int flagComment = 0;

        //Переменная для фамилии
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

        //Переменная для имени
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

        //Переменная для картинки профиля
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

        //Переменная для даты публикации
        private string datePublication;
        public string DatePublication
        {
            get
            {
                return datePublication;
            }
            set
            {
                Set(ref datePublication, value);
            }
        }

        //Переменная для текста комментируемого поста
        private string textPost;
        public string TextPost
        {
            get
            {
                return textPost;
            }
            set
            {
                Set(ref textPost, value);
            }
        }

        //Переменная для комментария
        private string commentText;
        public string CommentText
        {
            get
            {
                return commentText;
            }
            set
            {
                Set(ref commentText, value);
                flagComment = 1;

                if (CommentText == "" || CommentText == " " || CommentText == null)
                    ValidationErrors["CommentText"] = "Неверный формат! Поле не может быть пустым";
                else
                    ValidationErrors["CommentText"] = null;
            }
        }

        //Переменная для списка комментариев
        private ObservableCollection<Comment> comments;
        public ObservableCollection<Comment> Comments
        {
            get
            {
                return comments;
            }
            set
            {
                Set(ref comments, value);
            }
        }

        //email владельца данного поста
        private string emailProfileOwner;

        //email пользователя который зашел комметировать пост
        private string emailCurrentUser;

        //Переменная данного поста
        private Post currentPost;

        //Путь к стандартной картинке профиля
        private string standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");

        //Менеджер картинки профиля
        private ProfileImageManager imageManager;
        Dictionary<string, string> ValidationErrors;
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public CommentPostViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            ValidationErrors = new Dictionary<string, string>();
            Comments = new ObservableCollection<Comment>();
            imageManager = new ProfileImageManager();

            GoToProfile = new DelegateCommand(ExecuteGoToProfile, CanGoToProfile);
            GoSendComment = new DelegateCommand(ExecuteGoSendComment, CanGoSendComment);
        }
        #endregion


        #region Methods

        //Command для кнопки Назад---------------------------------------------------
        public ICommand GoToProfile { get; }

        private void ExecuteGoToProfile(object obj)
        {

            if(emailCurrentUser == emailProfileOwner)
            {
                navigationManager.Register(NavigationKeys.ProfileViewKey);
                navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailProfileOwner);         //Переход к user control Профиля владельца поста
            }
            else
            {
                navigationManager.Register(NavigationKeys.StrangeProfileViewKey);
                navigationManager.Navigate(NavigationKeys.StrangeProfileViewKey, new object[] { emailCurrentUser, emailProfileOwner });         //Переход к user control Профиля владельца поста
            }
        }
        private bool CanGoToProfile(object obj)
        {
            return true;
        }

        //------------------------------------------------------------------------------------

        //Command для кнопки Отправить---------------------------------------------------
        public ICommand GoSendComment{ get; }

        private void ExecuteGoSendComment(object obj)
        {
            try
            {
                using (UnitOfWork unit = new UnitOfWork())
                {
                    int flag = 0;
                    IEnumerable<Post> posts = unit.PostRepos.GetItems();
                    foreach(var post in posts)
                    {
                        if (post.Id == currentPost.Id)
                            flag = 1;
                    }

                    if(flag == 0)
                    {
                        if (emailCurrentUser == emailProfileOwner)
                        {
                            navigationManager.Register(NavigationKeys.ProfileViewKey);
                            navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailProfileOwner);         //Переход к user control Профиля владельца поста
                        }
                        else
                        {
                            navigationManager.Register(NavigationKeys.StrangeProfileViewKey);
                            navigationManager.Navigate(NavigationKeys.StrangeProfileViewKey, new object[] { emailCurrentUser, emailProfileOwner });         //Переход к user control Профиля владельца поста
                        }
                    }
                    var newComment = new Comment { PostId = currentPost.Id, Text = CommentText, UserEmail = emailCurrentUser, SendDate = DateTime.Now.ToString() };
                
                    unit.CommentRepos.AddItem(newComment);
                    unit.Save();

                    Comments = new ObservableCollection<Comment>(unit.CommentRepos.GetItems().Where(c => c.PostId == currentPost.Id).Select(t => t));
                }

                foreach (var comment in Comments)
                {
                    //Если у пользователя нет картинки профиля, устанавливается стандартная
                    if (comment.User.ImageId == null)
                        comment.User.ImageName = standardProfileImageName;
                    else
                    {
                        imageManager = new ProfileImageManager();
                        imageManager.RegisterFileStorage(comment.User.Email);
                        imageManager.ManagementPC(comment.User.ImageId, comment.User.ImageName);
                        if (!File.Exists(comment.User.ImageName))
                            comment.User.ImageName = standardProfileImageName;
                    }
                }

                CommentText = "";
            }
            catch
            {

            }
        }
        private bool CanGoSendComment(object obj)
        {
            if (flagComment == 0)
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
                var arrayInfo = obj as object[];
                if (arrayInfo == null)
                    return;

                emailCurrentUser = arrayInfo[0] as string;
                emailProfileOwner = arrayInfo[1] as string;
                currentPost = arrayInfo[2] as Post;

                Comments = new ObservableCollection<Comment>();


                DatePublication = currentPost.DatePublication;
                TextPost = currentPost.Text;

                using (UnitOfWork unit = new UnitOfWork())
                {
                    var user = unit.UserDataRepos.GetItem(new object[] { emailProfileOwner });
                    LastName = user.LastName;
                    Name = user.Name;
                    //Загрузка картинки профиля-----------------
                    if (user.ImageId == null)
                        ImageSource = standardProfileImageName;
                    else
                    {
                        if (File.Exists(user.ImageName))
                            ImageSource = user.ImageName;
                        else
                            ImageSource = standardProfileImageName;
                    }
                    //-----------------------------------------
                    Comments = new ObservableCollection<Comment>(unit.CommentRepos.GetItems().Where(c => c.PostId == currentPost.Id).Select(t => t));

                    //Просмотр всех пользователей, если у кого-то нет картинки профиля, установка стандартной
                    for (int i = 0; i < Comments.Count; i++)
                    {
                        if (Comments[i].User.ImageId == null)
                            Comments[i].User.ImageName = standardProfileImageName;
                        else
                        {
                            imageManager = new ProfileImageManager();
                            imageManager.RegisterFileStorage(Comments[i].User.Email);
                            imageManager.ManagementPC(Comments[i].User.ImageId, Comments[i].User.ImageName);
                            if (!File.Exists(Comments[i].User.ImageName))

                                Comments[i].User.ImageName = standardProfileImageName;
                        }
                    }
                }
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
        #endregion
    }
}
