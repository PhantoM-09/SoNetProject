using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.View.RightInformationView.Admin;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace curs.ViewModels.LoginRegistration
{
    public class LoginViewModel : BaseViewModel, IDataErrorInfo, INavigationAware
    {

        #region Fields
        private int flagLogin = 0;                                //Флаг для закрытия кнопки вначале программы
        private int flagPassword = 0;                                //Флаг для закрытия кнопки вначале программы

        private string e_mail;
        public string E_mail                                 //Свойство для связи с textbox для ввода e-mail
        {
            get
            {
                return e_mail;
            }
            set
            {
                TextTextBlock = "";
                //Установка значения
                Set(ref e_mail, value);
                flagLogin = 1;

                //Проверка на валидность с последующей записью сообщения_об_ошибке/null в словарь по ключу из свойства, которые проверяются на валидацию
                string pattern = @"^([a-z0-9_-]+\.)*[a-z0-9_-]+@mail.ru$";
                if (!Regex.IsMatch(E_mail, pattern))
                    ValidationErrors["E_mail"] = "Не верно введен E-mail!";
                else
                    ValidationErrors["E_mail"] = null;

            }
        }

        private string password;
        public string Password                               //Свойство для связи с textbox для ввода пароля
        {
            get
            {
                return password;
            }
            set
            {
                TextTextBlock = "";
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

        //Дополнительное свойство для сообщения о том, что есть несовпадение Email или Пароля
        private string textTextBlock;
        public string TextTextBlock
        {
            get
            {
                return textTextBlock;
            }
            set
            {
                Set(ref textTextBlock, value);
            }
        }

        Dictionary<string, string> ValidationErrors;         //Словарь с сообщениями об ошибках при вводе данных, ключом выступает свойство, привязанное к определенному texbox

        private readonly INavigationManager navigationManager;          //Навигатор для перемещения ГЛАВНЫХ user controls в контенте ГЛАВНОГО окна
        private readonly INavigationManager globalNavigationManager;    //Навигатор для перемещения user controls в контенте одного из ГЛАВНЫХ user controls

        #endregion

        #region Ctors
        public LoginViewModel(INavigationManager navigationManager, INavigationManager globalNavigationManager)
        {
            this.navigationManager = navigationManager;
            this.globalNavigationManager = globalNavigationManager;

            ValidationErrors = new Dictionary<string, string>();

            LoginToApplication = new DelegateCommand(ExecuteLoginToApplication, CanLoginToApplication);
            GoToRegistration = new DelegateCommand(ExecuteGoToRegistration, CanGoToRegistration);
        }
        #endregion

        #region Methods

        #region IDataErrorInfo
        //Реализация интерфейса IDataErrorInfo
        public string Error => throw new NotImplementedException();

        //Если есть данные по данному ключу- взять их, иначе - null
        public string this[string columnName] => ValidationErrors.ContainsKey(columnName) ? ValidationErrors[columnName] : null;
        #endregion


        //Command для кнопки Вход---------------------------------------------------
        public ICommand LoginToApplication { get; }

        private void ExecuteLoginToApplication(object obj)
        {
            try
            {

                if (E_mail == AdminKeys.AdminEmail && Password == AdminKeys.AdminPassword)
                {
                    globalNavigationManager.Register(NavigationKeys.AdminContextRightInfoViewKey);
                    globalNavigationManager.Navigate(NavigationKeys.AdminContextRightInfoViewKey, AdminKeys.AdminEmail);       //Переход ко второму из ГЛАВНЫХ user controls в контенте главного окна
                }
                else
                {
                    using (UnitOfWork unit = new UnitOfWork())
                    {
                        IEnumerable<UserData> users = unit.UserDataRepos.GetItems();
                        foreach(var user in users)
                        {
                            if(user.Email == E_mail && user.Password == GetHash(Password))
                            {
                                if(user.IsOnline == false)
                                {
                                    if(user.IsBlocked != true)
                                    {
                                        globalNavigationManager.Register(NavigationKeys.ContextRightInfoViewKey);
                                        globalNavigationManager.Navigate(NavigationKeys.ContextRightInfoViewKey, E_mail);       //Переход ко второму из ГЛАВНЫХ user controls в контенте главного окна
                                    }
                                    else
                                    {
                                        TextTextBlock = "Извините, но вы были заблокированы.";
                                        return;
                                    }
                                }
                                else
                                {
                                    TextTextBlock = "Пользователь уже в сети!";
                                    return;
                                }
                            }
                        }
                        TextTextBlock = "Неверно введен Email или Пароль!";
                    }
                }
            }
            catch
            {

            }
        }
        private bool CanLoginToApplication(object obj)
        {
            if (flagLogin == 0 || flagPassword == 0)
                return false;
            else
                return IsValid();
        }
        //--------------------------------------------------------

        //Command для кнопки Регистрация---------------------------------------------------
        public ICommand GoToRegistration { get; }

        private void ExecuteGoToRegistration(object obj)
        {
            navigationManager.Navigate(NavigationKeys.RegisterViewKey);         //Переход к user control Регистрации
        }
        private bool CanGoToRegistration(object obj)
        {
            return true;
        }

        //--------------------------------------------------------

        public bool IsValid() => ValidationErrors.Values.All(x => x == null);       //Если в словаре нет сообщений об ошибках - true, иначе - false

        //Интерфейс INavigationAware------------------------------
        public void WantDoSomethingBeforeClose()
        {
            
        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            try
            {
                //Перед запуском программы проверяется есть ли учетная запись Администратора, если нет-создается
                using (UnitOfWork unit = new UnitOfWork())
                {
                    //Берем всех пользователей из БД
                    IEnumerable<UserData> users = unit.UserDataRepos.GetItems();
                    if (!users.Any(u => u.Email == AdminKeys.AdminEmail && u.Password == AdminKeys.AdminPassword))
                    {
                        var user = new UserData()
                        {
                            Email = AdminKeys.AdminEmail,
                            Password = AdminKeys.AdminPassword
                        };
                        unit.UserDataRepos.AddItem(user);
                        unit.Save();
                    }
                }
                //---------------------------------------------------------------------------------------------

                //if(Directory.Exists())
            }
            catch
            {
            }
            
        }
        //-------------------------------------------------------

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
