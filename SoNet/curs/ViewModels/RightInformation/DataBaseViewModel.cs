using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.View.RightInformationView.Admin;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation
{
    public class DataBaseViewModel : BaseViewModel, INavigationAware, IDataErrorInfo
    {
        #region Fields and Properties
        private int flagEmail = 0;

        private int flagSearchFriends = 0;
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
        private string emailDeletedUser;
        public string EmailDeletedUser
        {
            get
            {
                return emailDeletedUser;
            }
            set
            {
                Set(ref emailDeletedUser, value);
                flagEmail = 1;
                //Проверка на валидность с последующей записью сообщения_об_ошибке/null в словарь по ключу из свойства, которые проверяются на валидацию
                string pattern = @"^([a-z0-9_-]+\.)*[a-z0-9_-]+@mail.ru$";
                if (!Regex.IsMatch(EmailDeletedUser, pattern))
                    ValidationErrors["EmailDeletedUser"] = "Не верно введен E-mail!";
                else
                    ValidationErrors["EmailDeletedUser"] = null;
            }
        }
        //Коллекция пользователей
        private ObservableCollection<UserData> tempUserCollection;
        private ObservableCollection<UserData> users;
        public ObservableCollection<UserData> Users
        {
            get
            {
                return users;
            }
            set
            {
                Set(ref users, value);
            }
        }

        Dictionary<string, string> ValidationErrors;
        private string emailCurrentUser;
        private NavigationManager navigationManager;

        #endregion

        #region Ctors
        public DataBaseViewModel(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            Users = new ObservableCollection<UserData>();
            ValidationErrors = new Dictionary<string, string>();
            GoToDelete = new DelegateCommand(ExecuteGoToDelete, CanGoToDelete);
            GoToBlock = new DelegateCommand(ExecuteGoToBlock, CanGoToBlock);
            GoToUnBlock = new DelegateCommand(ExecuteGoToUnBlock, CanGoToUnBlock);
            GoSearchFriends = new DelegateCommand(ExecuteGoSearchFriends, CanGoSearchFriends);
            GoCancelSearchFriends = new DelegateCommand(ExecuteGoCancelSearchFriends, CanGoCancelSearchFriends);
        }
        #endregion


        //Command для кнопки Удалить---------------------------------------------------
        public ICommand GoToDelete { get; }

        private void ExecuteGoToDelete(object obj)
        {
            try
            {
                if (EmailDeletedUser != AdminKeys.AdminEmail)
                {
                    using (UnitOfWork unit = new UnitOfWork())
                    {
                        //Удаление комментариев пользователя
                        var userComments = unit.CommentRepos.GetItems().Where(u => u.UserEmail == emailDeletedUser).Select(t => t);
                        foreach (var comment in userComments)
                        {
                            unit.CommentRepos.DeleteItem(new object[] { comment.Id });
                        }

                        //Удаление постов пользователя
                        var userPosts = unit.PostRepos.GetItems().Where(u => u.UserEmail == emailDeletedUser).Select(t => t);
                        foreach (var post in userPosts)
                        {
                            unit.PostRepos.DeleteItem(new object[] { post.Id });
                        }

                        //Удаление файлов пользователя
                        var userFiles = unit.UserFileRepos.GetItems().Where(u => u.UserEmail == emailDeletedUser).Select(t => t);
                        foreach (var file in userFiles)
                        {
                            unit.UserFileRepos.DeleteItem(new object[] { file.Id });
                        }

                        //Удаление друзей пользователя (1)
                        var userFriends1 = unit.FriendRepos.GetItems().Where(u => u.UserEmail == emailDeletedUser).Select(t => t);
                        foreach (var friend in userFriends1)
                        {
                            unit.FriendRepos.DeleteItem(new object[] { friend.UserEmail, friend.FriendEmail });
                        }

                        //Удаление друзей пользователя (2)
                        var userFriends2 = unit.FriendRepos.GetItems().Where(u => u.FriendEmail == emailDeletedUser).Select(t => t);
                        foreach (var friend in userFriends2)
                        {
                            unit.FriendRepos.DeleteItem(new object[] { friend.UserEmail, friend.FriendEmail });
                        }

                        unit.UserDataRepos.DeleteItem(new object[] { emailDeletedUser });
                        unit.Save();

                        Users = new ObservableCollection<UserData>(unit.UserDataRepos.GetItems());
                        Users.Remove(Users.FirstOrDefault(u => u.Email == AdminKeys.AdminEmail));
                    }

                }
                EmailDeletedUser = "";
            }
            catch
            {

            }
            
        }
        private bool CanGoToDelete(object obj)
        {
            if(flagEmail == 0)
                return false;
            return IsValid();
        }

        //--------------------------------------------------------

        //Command для кнопки Заблокировать---------------------------------------------------
        public ICommand GoToBlock { get; }

        private void ExecuteGoToBlock(object obj)
        {
            if(EmailDeletedUser != AdminKeys.AdminEmail)
            {
                using(UnitOfWork unit = new UnitOfWork())
                {
                    var user = unit.UserDataRepos.GetItem(new object[] { emailDeletedUser });
                    user.IsBlocked = true;

                    unit.Save();

                    Users = new ObservableCollection<UserData>(unit.UserDataRepos.GetItems());
                    Users.Remove(Users.FirstOrDefault(u => u.Email == AdminKeys.AdminEmail));
                }
                EmailDeletedUser = "";
            }
        }
        private bool CanGoToBlock(object obj)
        {
            if (flagEmail == 0)
                return false;
            return IsValid();
        }

        //--------------------------------------------------------


        //Command для кнопки Разблокировать---------------------------------------------------
        public ICommand GoToUnBlock { get; }

        private void ExecuteGoToUnBlock(object obj)
        {
            if (EmailDeletedUser != AdminKeys.AdminEmail)
            {
                using (UnitOfWork unit = new UnitOfWork())
                {
                    var user = unit.UserDataRepos.GetItem(new object[] { emailDeletedUser });
                    user.IsBlocked = false;

                    unit.Save();

                    Users = new ObservableCollection<UserData>(unit.UserDataRepos.GetItems());
                    Users.Remove(Users.FirstOrDefault(u => u.Email == AdminKeys.AdminEmail));
                }
                EmailDeletedUser = "";
            }
        }
        private bool CanGoToUnBlock(object obj)
        {
            if (flagEmail == 0)
                return false;
            return IsValid();
        }

        //--------------------------------------------------------

        //Command для поиска из списка , также здесь и отмена этого поиска------------------------------------
        public ICommand GoSearchFriends { get; }

        private void ExecuteGoSearchFriends(object obj)
        {
            if (flagSearchFriends == 0)
                tempUserCollection = Users;

            Users = new ObservableCollection<UserData>(tempUserCollection.Where(f => f.LastName == SearchRequest).Select(t => t));
            flagSearchFriends = 1;
        }
        private bool CanGoSearchFriends(object obj)
        {
            return true;
        }

        public ICommand GoCancelSearchFriends { get; }

        private void ExecuteGoCancelSearchFriends(object obj)
        {
            Users = tempUserCollection;
            SearchRequest = "";
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

        public void WantDoSomethingBeforeClose()
        {
            
        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            try
            {
                string email = obj as string;
                if (email == null)
                    return;
                emailCurrentUser = email;

                using(UnitOfWork unit = new UnitOfWork())
                {
                    Users = new ObservableCollection<UserData>(unit.UserDataRepos.GetItems());
                    Users.Remove(Users.FirstOrDefault(u => u.Email == AdminKeys.AdminEmail));
                }
            }
            catch
            {

            }
        }

        #region IDataErrorInfo
        //Реализация интерфейса IDataErrorInfo
        public string Error => throw new NotImplementedException();

        //Если есть данные по данному ключу- взять их, иначе - null
        public string this[string columnName] => ValidationErrors.ContainsKey(columnName) ? ValidationErrors[columnName] : null;
        #endregion

        public bool IsValid() => ValidationErrors.Values.All(x => x == null);
    }
}
