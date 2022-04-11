using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace curs.ViewModels.LoginRegistration
{
    public class RegisterViewModel : BaseViewModel, INavigationAware, IDataErrorInfo
    {

        #region Fields

        private int flagLastName = 0;
        private int flagName = 0;
        private int flagLogin = 0;
        private int flagPassword = 0;

        //Поле для фамилии
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
                flagLastName = 1;

                //Проверка на валидность с последующей записью сообщения_об_ошибке/null в словарь по ключу из свойства, которые проверяются на валидацию
                string pattern = @"^[а-яА-Я]+${1,20}";
                if (!Regex.IsMatch(LastName, pattern))
                    ValidationErrors["LastName"] = "Не верно введена фамилия!";
                else
                    ValidationErrors["LastName"] = null;
            }
        }

        //Поле для имени
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
                flagName = 1;
                //Проверка на валидность с последующей записью сообщения_об_ошибке/null в словарь по ключу из свойства, которые проверяются на валидацию
                string pattern = @"^[а-яА-Я]+${1,20}";
                if (!Regex.IsMatch(Name, pattern))
                    ValidationErrors["Name"] = "Не верно введено имя!";
                else
                    ValidationErrors["Name"] = null;
            }
        }

        //Поле для почты
        private string e_mail;
        public string E_mail                                 //Свойство для связи с textbox для ввода e-mail
        {
            get
            {
                return e_mail;
            }
            set
            {
                //Установка значения
                Set(ref e_mail, value);
                flagLogin = 1;

                //Проверка на валидность с последующей записью сообщения_об_ошибке/null в словарь по ключу из свойства, которые проверяются на валидацию
                string pattern = @"^([a-z0-9_-]+\.)*[a-z0-9_-]+@mail.ru$";
                if (!Regex.IsMatch(E_mail, pattern))
                    ValidationErrors["E_mail"] = "Не верно введен E-mail!";
                else
                {
                    using (UnitOfWork unit = new UnitOfWork())
                    {
                        IEnumerable<UserData> users = unit.UserDataRepos.GetItems();
                        if (users.Any(u => u.Email == E_mail))
                            ValidationErrors["E_mail"] = "Такой Email уже есть!";
                        else
                            ValidationErrors["E_mail"] = null;
                    }

                }
            }
        }

        //Поле для пароля
        private string password;
        public string Password                               //Свойство для связи с textbox для ввода пароля
        {
            get
            {
                return password;
            }
            set
            {
                //Установка значения
                Set(ref password, value);
                flagPassword = 1;
                //Проверка на валидность с последующей записью сообщения/null в словарь по ключу из свойства
                string pattern = @"^[a-z0-9]{1,10}$";
                if (!Regex.IsMatch(Password, pattern))
                    ValidationErrors["Password"] = "Не верно введен пароль!";
                else
                    ValidationErrors["Password"] = null;
            }
        }

        Dictionary<string, string> ValidationErrors;
        private readonly INavigationManager navigationManager;                  //Навигатор для перемещения user controls в контенте одного из ГЛАВНЫХ user controls

        #endregion

        #region Ctors

        public RegisterViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            ValidationErrors = new Dictionary<string, string>();

            GoInputOtherInfo = new DelegateCommand(ExecuteGoInputOtherInfo, CanGoInputOtherInfo);
            GoToLogin = new DelegateCommand(ExecuteGoToLogin, CanGoToLogin);
        }

        #endregion

        #region Methods

        //Command для кнопки Далее------------------------------------
        public ICommand GoInputOtherInfo { get; }

        private void ExecuteGoInputOtherInfo(object obj)
        {
            ICollection<object> firstPartUserInfo = new List<object>();
            firstPartUserInfo.Add(LastName);
            firstPartUserInfo.Add(Name);
            firstPartUserInfo.Add(E_mail);
            firstPartUserInfo.Add(GetHash(Password));

            navigationManager.Navigate(NavigationKeys.RegisterEndViewKey, firstPartUserInfo);
        }
        private bool CanGoInputOtherInfo(object obj)
        {
            if (flagLastName == 0 || flagName == 0 || flagLogin == 0 || flagPassword == 0)
                return false;
            else
                return IsValid();
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
            if (E_mail != null)
            {
                E_mail = "";
            }
        }
        //------------------------------------------------------------------------------------------------------------------------

        //Интерфейс IDataErrorInfo------------------------------------------------------------------------------------------------
        public string Error => throw new NotImplementedException();

        public string this[string columnName] => ValidationErrors.ContainsKey(columnName) ? ValidationErrors[columnName] : null;
        //------------------------------------------------------------------------------------------------------------------------

        //Проверка валидации формы
        public bool IsValid() => ValidationErrors.Values.All(x => x == null);

        //Хеширование пароля
        public string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }
        #endregion
    }
}
