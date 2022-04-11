using curs.Infrastructure.Commands.Base;
using curs.Infrastructure.Navigation;
using curs.ViewModels.Base;
using DatabaseClasses.UnitOfWorkPattern;
using FileManagement;
using Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace curs.ViewModels.RightInformation
{
    public class MyFilesViewModel : BaseViewModel, INavigationAware
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
        private INavigationManager navigationManager;
        #endregion

        #region Ctors
        public MyFilesViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            fileManager = new FileManager();
            fileManager.RegisterFileStorage(emailCurrentUser);
            Files = new ObservableCollection<UserFile>();
            tempFileCollection = new ObservableCollection<UserFile>();

            GoSearchFiles = new DelegateCommand(ExecuteGoSearchFiles, CanGoSearchFiles);
            GoCancelSearchFiles = new DelegateCommand(ExecuteGoCancelSearchFiles, CanGoCancelSearchFiles);
            GoAddFiles = new DelegateCommand(ExecuteGoAddFiles, CanGoAddFiles);
            GoDeleteFile = new DelegateCommand(ExecuteGoDeleteFile, CanGoDeleteFile);
            GoDownloadFile = new DelegateCommand(ExecuteGoDownloadFile, CanGoDownloadFile);
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

        //Command для поиска из списка файлов, также здесь и отмена этого поиска------------------------------------
        public ICommand GoAddFiles { get; }

        private void ExecuteGoAddFiles(object obj)
        {
            try
            {
                GoCancelSearchFiles.Execute(new object());
                string selectedFile;
                selectedFile = navigationManager.AddFile();

                if (selectedFile == "")
                    return;
                
                string[] fileIdName = fileManager.ManagementDisk(selectedFile);
                if (fileIdName == null)
                    return;

                string fileId = fileIdName[0];
                string fileName = fileIdName[1];

                var fileInfo = new UserFile() { FileID = fileId, FileName = fileName, UserEmail = emailCurrentUser };
                using(UnitOfWork unit = new UnitOfWork())
                {
                    unit.UserFileRepos.AddItem(fileInfo);
                    unit.Save();
                }
                Files.Add(fileInfo);
            }
            catch
            {

            }
           


        }
        private bool CanGoAddFiles(object obj)
        {
            return true;
        }
        //----------------------------------------------------------

        //Команда для скачивания файла из коллекции файлов пользователя
        public ICommand GoDownloadFile { get; }

        private void ExecuteGoDownloadFile(object obj)
        {
            try
            {
                var file = obj as UserFile;
                if (file == null)
                    return;

                string fileName = navigationManager.DownloadFile();
                if (fileName == "")
                    return;

                fileManager.ManagementPC(file.FileID, fileName);
            }
            catch
            {

            }

        }
        private bool CanGoDownloadFile(object obj)
        {
            return true;
        }
        //-----------------------------------------------------------

        //Команда для удаления файла из коллекции файлов пользователя
        public ICommand GoDeleteFile { get; }

        private void ExecuteGoDeleteFile(object obj)
        {
            try
            {
                var file = obj as UserFile;
                if (file == null)
                    return;

                //Удаление файла с диска
                fileManager.DeleteFileFromDisk(file.FileID);

                //Удаление файла из БД
                using (UnitOfWork unit = new UnitOfWork())
                {
                    var deletedFile = unit.UserFileRepos.GetItems().Where(f => f.FileID == file.FileID).First();
                    unit.UserFileRepos.DeleteItem(new object[] { deletedFile.Id });
                    unit.Save();
                }

                //Удаление файла с коллекции представления
                Files.Remove(file);
            }
            catch
            {

            }
            
        }
        private bool CanGoDeleteFile(object obj)
        {
                return true;
        }
        //-----------------------------------------------------------

        //Реализация интерфейса INavigationAware--------------------------------
        public void WantDoSomethingBeforeClose()
        {
            fileManager.ToNull();
        }

        public void WantDoSomethingBeforeOpen(object obj = null)
        {
            var email = obj as string;
            if (email == null)
                return;

            emailCurrentUser = email;
            fileManager.RegisterFileStorage(email);

            Files = new ObservableCollection<UserFile>();
            var files = fileManager.GetFileCollection();

            foreach(var file in files)
            {
                UserFile userFile = new UserFile() { UserEmail = email, FileName = file.Key, FileID = file.Value };
                Files.Add(userFile);
            }

            tempFileCollection = Files;
        }
        //----------------------------------------------------------------------
        #endregion
    }
}
