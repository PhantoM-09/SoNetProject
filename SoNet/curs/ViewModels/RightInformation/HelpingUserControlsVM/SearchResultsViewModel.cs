using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation.HelpingUserControlsVM
{
    class SearchResultsViewModel : BaseViewModel, INavigationAware
    {
        #region Fields and Properties

        //Путь к стандартной картинке профиля
        private string standardProfileImageName = Path.GetFullPath("../../Resources/StandardProfileImage.png");

        //Количество найденых людей
        private int countFindPeople;
        public int CountFindPeople
        {
            get
            {
                return countFindPeople;
            }
            set
            {
                Set(ref countFindPeople, value);
            }
        }

        //Переменная для хранения выбранного пользователя
        private UserData selectedUser;
        public UserData SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                Set(ref selectedUser, value);
                if (SelectedUser != null)
                {
                    OpenStrangeProfile.Execute(SelectedUser.Email);
                }
            }
        }


        //Коллекци содержащая найденых людей
        private ObservableCollection<UserData> peoples;
        public ObservableCollection<UserData> Peoples
        {
            get
            {
                return peoples;
            }
            set
            {
                Set(ref peoples, value);
            }
        }

        private string emailCurrentUser;
        private ProfileImageManager imageManager;
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public SearchResultsViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            imageManager = new ProfileImageManager();
            Peoples = new ObservableCollection<UserData>();

            OpenStrangeProfile = new DelegateCommand(ExecuteOpenStrangeProfile, CanOpenStrangeProfile);
        }
        #endregion


        #region Methods

        //Command для элемента listbox, открытие чужого профиля------------------------------------
        public ICommand OpenStrangeProfile { get; }

        private void ExecuteOpenStrangeProfile(object obj)
        {
            navigationManager.Register(NavigationKeys.StrangeProfileViewKey);
            navigationManager.Navigate(NavigationKeys.StrangeProfileViewKey, new object[] {emailCurrentUser,  obj as string });
        }
        private bool CanOpenStrangeProfile(object obj)
        {
            return true;
        }

        //----------------------------------------------------------

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
                string searchRequest = array[1] as string;

                using(UnitOfWork unit = new UnitOfWork())
                {
                    Peoples = new ObservableCollection<UserData>(unit.UserDataRepos.GetItems().Where(u => u.LastName == searchRequest).Select(t=>t));
                }
                var us = Peoples.Where(u => u.Email == emailCurrentUser)?.First();
                Peoples.Remove(us);
                CountFindPeople = Peoples.Count;
                for(int i = 0; i<Peoples.Count; i++)
                {

                    if (Peoples[i].ImageId == null)
                        Peoples[i].ImageName = standardProfileImageName;
                    else
                    {
                        imageManager = new ProfileImageManager();
                        imageManager.RegisterFileStorage(Peoples[i].Email);
                        imageManager.ManagementPC(Peoples[i].ImageId, Peoples[i].ImageName);
                        if (!File.Exists(Peoples[i].ImageName))

                            Peoples[i].ImageName = standardProfileImageName;
                    }
                }
            }
            catch
            {
                CountFindPeople = Peoples.Count;
                for (int i = 0; i < Peoples.Count; i++)
                {

                    if (Peoples[i].ImageId == null)
                        Peoples[i].ImageName = standardProfileImageName;
                    else
                    {
                        imageManager = new ProfileImageManager();
                        imageManager.RegisterFileStorage(Peoples[i].Email);
                        imageManager.ManagementPC(Peoples[i].ImageId, Peoples[i].ImageName);
                        if (!File.Exists(Peoples[i].ImageName))

                            Peoples[i].ImageName = standardProfileImageName;
                    }
                }
            }
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
