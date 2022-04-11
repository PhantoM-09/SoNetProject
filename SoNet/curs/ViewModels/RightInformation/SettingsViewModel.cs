using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation
{
    class SettingsViewModel : BaseViewModel, INavigationAware, IDataErrorInfo
    {
        #region Fields and Properties
        private int flagLastName = 0;
        private int flagName = 0;
        private int flagSex = 0;
        private int flagCountry = 0;

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

        //Значение для Image_Source
        private string imageName;
        public string ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                Set(ref imageName, value);
            }
        }

        private string standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");
        //Менеджер картинки профиля
        private ProfileImageManager imageManager = new ProfileImageManager();

        private string emailCurrentUser;
        Dictionary<string, string> ValidationErrors;
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public SettingsViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            ValidationErrors = new Dictionary<string, string>();

            GoToSaveChanges = new DelegateCommand(ExecuteGoToSaveChanges, CanGoToSaveChanges);
           
        }
        #endregion


        #region Methods

        //Command для кнопки Принять---------------------------------------------------
        public ICommand GoToSaveChanges { get; }

        private void ExecuteGoToSaveChanges(object obj)
        {
            try
            {
                using(UnitOfWork unit = new UnitOfWork())
                {
                    var user = unit.UserDataRepos.GetItem(new object[] { emailCurrentUser });

                    user.LastName = LastName;
                    user.Name = Name;
                    user.Sex = Sex;
                    user.BirthDay = BirthDay;
                    user.Country = Country;

                    unit.UserDataRepos.UpdateItem(user);
                    unit.Save();
                }
                navigationManager.Register(NavigationKeys.ProfileViewKey);
                navigationManager.Navigate(NavigationKeys.ProfileViewKey, emailCurrentUser);
            }
            catch
            {

            }
        }
        private bool CanGoToSaveChanges(object obj)
        {
            if (flagLastName == 0 && flagName == 0 && flagSex == 0 && flagCountry == 0)
                return false;
            else
                return IsValid();
        }

        //--------------------------------------------------------

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

        public void WantDoSomethingBeforeOpen(object obj= null)
        {
            try
            {
                var email = obj as string;
                if (email == null)
                    return;

                emailCurrentUser = email;

                
                using(UnitOfWork unit = new UnitOfWork())
                {
                    var user = unit.UserDataRepos.GetItem(new object[] { email });

                    LastName = user.LastName;
                    Name = user.Name;
                    Sex = user.Sex;
                    BirthDay = user.BirthDay;
                    Country = user.Country;

                    if (user.ImageId == null)
                        ImageName = standardProfileImageName;
                    else
                    {
                        imageManager = new ProfileImageManager();
                        imageManager.RegisterFileStorage(email);
                        if (!File.Exists(user.ImageName))
                        {
                            imageManager.ManagementPC(user.ImageId, user.ImageName);
                            if(!File.Exists(user.ImageName))
                            {
                                ImageName = standardProfileImageName;
                            }
                            else
                            {
                                ImageName = user.ImageName;
                            }
                        }
                        else
                        {
                            ImageName = user.ImageName;
                        }
                          
                    }
                }
            }
            catch
            {
                ImageName = standardProfileImageName;
            }
            

        }
        //----------------------------------------------------------------------
        #endregion
    }
}
