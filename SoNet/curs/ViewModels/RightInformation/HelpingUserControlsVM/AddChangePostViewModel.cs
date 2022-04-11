using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Models;
using DatabaseClasses.UnitOfWorkPattern;

namespace curs.ViewModels.RightInformation.HelpingUserControlsVM
{
    class AddChangePostViewModel : BaseViewModel, INavigationAware, IDataErrorInfo
    {
        #region Fields and Properties

        private Post changedPost;
        private int flagNewComment = 0;

        //Переменная для нового комментария
        private string newTextPost;
        public string NewTextPost
        {
            get
            {
                return newTextPost;
            }
            set
            {
                Set(ref newTextPost, value);
                flagNewComment = 1;

                if (NewTextPost == "" || NewTextPost == " " || NewTextPost == null)
                    ValidationErrors["NewComment"] = "Неверный формат! Поле не может быть пустым";
                else
                    ValidationErrors["NewComment"] = null;
            }
        }

        //Переменная для старого комментария
        private string oldTextPost;
        public string OldTextPost
        {
            get
            {
                return oldTextPost;
            }
            set
            {
                Set(ref oldTextPost, value);
            }
        }

        //Email пользователя, создавшего пост
        private string emailCurrentUser;

        Dictionary<string, string> ValidationErrors;
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public AddChangePostViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            ValidationErrors = new Dictionary<string, string>();

            GoToProfile = new DelegateCommand(ExecuteGoToProfile, CanGoToProfile);
            SaveComment = new DelegateCommand(ExecuteSaveComment, CanSaveComment);
        }
        #endregion


        #region Methods


        //Command для кнопки Назад---------------------------------------------------
        public ICommand GoToProfile { get; }

        private void ExecuteGoToProfile(object obj)
        {
            navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);         //Переход к user control Профиль
        }
        private bool CanGoToProfile(object obj)
        {
            return true;
        }

        //Command для кнопки Принять---------------------------------------------------
        public ICommand SaveComment { get; }

        private void ExecuteSaveComment(object obj)
        {
            changedPost.Text = NewTextPost;
            using(UnitOfWork unit = new UnitOfWork())
            {
                unit.PostRepos.UpdateItem(changedPost);
                unit.Save();
            }
            navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);         //Переход к user control Профиль
        }
        private bool CanSaveComment(object obj)
        {
            if (flagNewComment == 0)
                return false;
            return IsValid();
        }

        //--------------------------------------------------------


        //Реализация интерфейса INavigationAware--------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            var arrayInfo = obj as object[];
            if (arrayInfo == null)
                return;

            emailCurrentUser = arrayInfo[0] as string;
            changedPost = arrayInfo[1] as Post;

            OldTextPost = changedPost.Text;
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
