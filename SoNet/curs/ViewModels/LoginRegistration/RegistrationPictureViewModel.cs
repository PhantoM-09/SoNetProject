using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using Models;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace curs.ViewModels.LoginRegistration
{
    class RegistrationPictureViewModel : BaseViewModel, INavigationAware
    {
        #region Fields

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


        //Три части информации о пользователе
        private object threePartUserInfo;

        //Путь к стандартной картинке профиля
        private string standardProfileImageName;

        //Id картинки профиля на диске
        private string profileImageId;
        //Путь к картинке профиля на ПК
        private string profileImageName;

        //Менеджер картинки профиля
        private ProfileImageManager imageManager = new ProfileImageManager();

        private readonly INavigationManager navigationManager;                  //Навигатор для перемещения user controls в контенте одного из ГЛАВНЫХ user controls
        private readonly INavigationManager globalNavigationManager;            //Навигатор для перемещения user controls в контенте одного из ГЛАВНЫХ user controls
        #endregion

        #region Ctors

        public RegistrationPictureViewModel(INavigationManager navigationManager, INavigationManager globalNavigationManager)
        {
            this.navigationManager = navigationManager;
            this.globalNavigationManager = globalNavigationManager;

            CreateNewAccount = new DelegateCommand(ExecuteCreateNewAccount, CanCreateNewAccount);
            GoToLogin = new DelegateCommand(ExecuteGoToLogin, CanGoToLogin);
            GoChooseImage = new DelegateCommand(ExecuteGoChooseImage, CanGoChooseImage);
        }
        #endregion

        #region Methods

        //Command для кнопки Регистрация------------------------------------
        public ICommand CreateNewAccount { get; }

        private void ExecuteCreateNewAccount(object obj)
        {
            try
            {
                if (ImageName != null)
                {
                    var listUserInfo = threePartUserInfo as List<object>;
                    if (listUserInfo != null)
                    {
                        listUserInfo.Add(profileImageName);
                        listUserInfo.Add(profileImageId);
                        using (UnitOfWork unit = new UnitOfWork())
                        {
                            var user = new UserData()
                            {
                                LastName = listUserInfo[0] as string,
                                Name = listUserInfo[1] as string,
                                Email = listUserInfo[2] as string,
                                Password = listUserInfo[3] as string,
                                BirthDay = listUserInfo[4] as string,
                                Sex = listUserInfo[5] as string,
                                Country = listUserInfo[6] as string,
                                ImageName = listUserInfo[7] as string,
                                ImageId = listUserInfo[8] as string,
                                IsBlocked = false,
                            };
                            unit.UserDataRepos.AddItem(user);
                            unit.Save();
                        }
                    }
                    globalNavigationManager.Navigate(NavigationKeys.ContextLoginRegisterKey);
                }
                
            }
            catch
            {
                navigationManager.Navigate(NavigationKeys.ErrorMessageViewKey, "Произошла ошибка добавления данных. Проверьте введеные данные");
            }
        }

        private bool CanCreateNewAccount(object obj)
        {
            return true;
        }
        //----------------------------------------------------------

        //Command для кнопки Вход------------------------------------
        public ICommand GoToLogin { get; }

        private void ExecuteGoToLogin(object obj)
        {
            //Так как папка создаётся при входе на страницу, то при выходе, не сохранив, ее необходимо удалить
            imageManager.DeleteFolder();

            globalNavigationManager.Navigate(NavigationKeys.ContextLoginRegisterKey);
        }
        private bool CanGoToLogin(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

        //Command для кнопки Выбрать картинку------------------------------------
        public ICommand GoChooseImage { get; }

        private void ExecuteGoChooseImage(object obj)
        {
            string choiceImageName = navigationManager.AddImage();              //Путь к выбраной картинке

            if (choiceImageName != "")
            {
                //Получаем id загруженной картинки
                profileImageId = imageManager.ManagementDisk(choiceImageName);
                
                ImageName = choiceImageName;
            }
        }
        private bool CanGoChooseImage(object obj)
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
            if (obj != null)
            {
                threePartUserInfo = obj;
                var listUserInfo = threePartUserInfo as List<object>;
                if (listUserInfo != null)
                {
                    //Регистрация менеджера картинки профиля
                    imageManager.RegisterFileStorage(listUserInfo[2] as string);

                    //Получаем путь где будет располагаться картинка профиля
                    profileImageName = imageManager.GetFutureImagePath();

                    //Установление поля для картинки в стандартную картинку
                    standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");
                    ImageName = standardProfileImageName;
                } 
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
