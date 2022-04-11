
using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace curs.ViewModels.LoginRegistration
{
    class RegisterEndViewModel : BaseViewModel, INavigationAware, IDataErrorInfo
    {
        #region Fields and Properties
        private int flagSex = 0;
        private int flagCountry = 0;

        //Две части информации о пользователе
        private object twoPartUserInfo;

        //Поле для даты рождения
        private string birthDay = DateTime.Now.ToShortDateString();
        public string BirthDay
        {
            get
            {
                return birthDay;
            }
            set
            {
                Set(ref birthDay, value);

                //Проверка на валидность с последующей записью сообщения_об_ошибке/null в словарь по ключу из свойства, которые проверяются на валидацию
                string pattern = @"(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d";
                if (!Regex.IsMatch(BirthDay, pattern))
                    ValidationErrors["BirthDay"] = "Не верно введена дата!";
                else
                    ValidationErrors["BirthDay"] = null;

            }
        }

        //Поле для пола
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
                flagSex = 1;

                if (sex == null)
                    ValidationErrors["Sex"] = "Не верно выбран пол!";
                else
                    ValidationErrors["Sex"] = null;
            }
        }

        //Поле для страны
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
                flagCountry = 1;

                if (country == null)
                    ValidationErrors["Country"] = "Не верно выбрана страна!";
                else
                    ValidationErrors["Country"] = null;
            }
        }

        Dictionary<string, string> ValidationErrors;
        private readonly INavigationManager navigationManager;                  //Навигатор для перемещения user controls в контенте одного из ГЛАВНЫХ user controls

        #endregion

        #region Ctors

        public RegisterEndViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            ValidationErrors = new Dictionary<string, string>();

            GoToPickImage = new DelegateCommand(ExecuteGoToPickImage, CanGoToPickImage);
            GoToLogin = new DelegateCommand(ExecuteGoToLogin, CanGoToLogin);
        }

        #endregion

        #region Methods

        //Command для кнопки Далее------------------------------------
        public ICommand GoToPickImage { get; }

        private bool CanGoToPickImage(object obj)
        {
            if (flagSex == 0 || flagCountry == 0)
                return false;
            else
                return IsValid();
        }

        private void ExecuteGoToPickImage(object obj)
        {
            var listInfo = twoPartUserInfo as ICollection<object>;
            if (listInfo != null)
            {
                listInfo.Add(BirthDay);
                listInfo.Add(Sex);
                listInfo.Add(Country);
            }

            navigationManager.Navigate(NavigationKeys.RegisterPictureViewKey, listInfo);
        }
        //----------------------------------------------------------

        //Command для кнопки Вход------------------------------------
        public ICommand GoToLogin { get; }

        private void ExecuteGoToLogin(object obj)
        {
            navigationManager.Navigate(NavigationKeys.LoginViewKey);
        }

        private bool CanGoToLogin(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Интерфейс INavigationAware---------------------------------------------------------------------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            if(obj !=null)
            {
                twoPartUserInfo = obj;

            }
        }
        //------------------------------------------------------------------------------------------------------------------------

        //Интерфейс IDataErrorInfo------------------------------------------------------------------------------------------------
        public string Error => throw new NotImplementedException();

        public string this[string columnName] => ValidationErrors.ContainsKey(columnName) ? ValidationErrors[columnName] : null;
        //------------------------------------------------------------------------------------------------------------------------

        //Проверка валидации формы
        public bool IsValid() => ValidationErrors.Values.All(x => x == null);

        #endregion
    }
}
