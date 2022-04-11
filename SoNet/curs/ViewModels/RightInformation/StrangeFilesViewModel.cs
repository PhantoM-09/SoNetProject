using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using FileManagement;
using Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation
{
    class StrangeFilesViewModel : BaseViewModel, INavigationAware
    {
        #region Fields and Properties

        private int flagSearchFile = 0;

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

        private ObservableCollection<UserFile> tempFileCollection;
        private ObservableCollection<UserFile> files;
        public ObservableCollection<UserFile> Files
        {
            get
            {
                return files;
            }
            set
            {
                Set(ref files, value);
            }
        }

        private FileManager fileManager;
        private string emailCurrentUser;
        private string emailOwnerFiles;
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public StrangeFilesViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            fileManager = new FileManager();
            fileManager.RegisterFileStorage(emailOwnerFiles);

            Files = new ObservableCollection<UserFile>();
            tempFileCollection = new ObservableCollection<UserFile>();

            GoSearchFiles = new DelegateCommand(ExecuteGoSearchFiles, CanGoSearchFiles);
            GoCancelSearchFiles = new DelegateCommand(ExecuteGoCancelSearchFiles, CanGoCancelSearchFiles);
            GoAddFile = new DelegateCommand(ExecuteGoAddFile, CanGoAddFile);
        }
        #endregion


        #region Methods

        //Command для поиска из списка файлов, также здесь и отмена этого поиска------------------------------------
        public ICommand GoSearchFiles { get; }

        private void ExecuteGoSearchFiles(object obj)
        {
            if (flagSearchFile == 0)
                tempFileCollection = Files;

            Files = new ObservableCollection<UserFile>(tempFileCollection.Where(f => f.FileName == SearchRequest).Select(t => t));
            flagSearchFile = 1;
        }
        private bool CanGoSearchFiles(object obj)
        {
            return true;
        }

        public ICommand GoCancelSearchFiles { get; }

        private void ExecuteGoCancelSearchFiles(object obj)
        {
            Files = tempFileCollection;
            SearchRequest = "";
            flagSearchFile = 0;
        }
        private bool CanGoCancelSearchFiles(object obj)
        {
            if (flagSearchFile == 0)
                return false;
            else
                return true;
        }

        //----------------------------------------------------------

        //Команда для добавления чужого файла к себе
        public ICommand GoAddFile { get; }

        private void ExecuteGoAddFile(object obj)
        {
            try
            {
                var file = obj as UserFile;
                fileManager.CopyFileInOtherFolder(file.FileName, file.FileID, emailCurrentUser);
            }
            catch
            {

            }

        }
        private bool CanGoAddFile(object obj)
        {
            return true;
        }
        //-----------------------------------------------------------

        //Реализация интерфейса INavigationAware--------------------------------
        public void WantDoSomethingBeforeClose()
        {

        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            var array = obj as object[];
            if (array == null)
                return;

            emailCurrentUser = array[0] as string;
            emailOwnerFiles = array[1] as string;
            fileManager.RegisterFileStorage(emailOwnerFiles);

            Files = new ObservableCollection<UserFile>();
            var files = fileManager.GetFileCollection();

            foreach (var file in files)
            {
                UserFile userFile = new UserFile() { UserEmail = emailOwnerFiles, FileName = file.Key, FileID = file.Value };
                Files.Add(userFile);
            }

            tempFileCollection = Files;
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
