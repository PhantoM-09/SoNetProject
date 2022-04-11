using DatabaseClasses.UnitOfWorkPattern;
using FileManagement.GoogleApi;
using Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FileManagement
{
    public class FileManager 
    {
        #region Fields
        private string diskMainFolder;                                                       //Главное хранилище папок и файлов пользователей на диске
        private string diskFolderCurrentUser;                                                //Папка текущего пользователя на диске. Создана на основе его Email

        private string emailCurrentUser;                                 //Email текущего пользователя для создания папок на диске и ПК
        private int fileNumber;                                      //Номер загруженного на диск файла
        private Dictionary<string, string> idFileByFileName;             //Ключ: имя файла на диске, значение: id файла на диске

        #endregion

        #region Ctors

        public FileManager()
        {
            idFileByFileName = new Dictionary<string, string>();
        }

        #endregion

        //Регистрация файлового хранилища
        public void RegisterFileStorage(string email)
        {
            try
            {
                //Создание, если это требуется, родительских папок
                CreateIfNotExists();
                if (email != null)
                {
                    //Сохранение email текущего владельца файловым менеджером
                    emailCurrentUser = email;

                    using (var unitOfWork = new UnitOfWork())
                    {
                        //Взятие информации про файлы, которые уже были загружены на диск
                        IEnumerable<UserFile> filesCurrentUser = unitOfWork.UserFileRepos.GetItems().Where(f => f.UserEmail == email).Select(f => f);

                        fileNumber = 0;
                        //Помещаем информацию о файлах в словарь
                        if (filesCurrentUser != null)
                        {
                            idFileByFileName = new Dictionary<string, string>();
                            foreach (var file in filesCurrentUser)
                            {
                                idFileByFileName[file.FileName] = file.FileID;
                                fileNumber++;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        #region Методы-менеджеры

        //Метод-менеджер по сбору данных и загрузке файла на диск
        public string[] ManagementDisk(string fileName)
        {
            try
            {
                string diskFileName = GetDiskFileName(fileName);                              //Получение имени файла на диске
                string contentTypeFile = GetContentTypeFile(fileName);                        //Получения contentType файла
                CreateFolderByEmailUserInDisk(emailCurrentUser);                              //Создание на диске папки конкретного юзера 

                if (diskFileName != null && contentTypeFile != null && diskFolderCurrentUser != null)
                {
                    string idFile = UploadFileOnDisk(diskFileName, fileName, contentTypeFile, diskFolderCurrentUser);
                    if(idFile != null)
                    {
                        idFileByFileName[diskFileName] = idFile;                              //Добавление имени и id загруженного файла в словарь
                        
                        return new string[] { idFile, diskFileName };
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            

        }

        //Метод-менеджер по сбору данных и скачиванию файла на ПК
        public void ManagementPC(string idFile, string fileName)
        {
            try
            {
                if(idFile != null && CreateFolderInPC(fileName))
                {
                    DownloadFileFromDisk(idFile, fileName);
                }
            }
            catch
            {

            }
        }

        public void CopyFileInOtherFolder(string fileName, string fileId, string emailWhoWant)
        {
            if(fileName != null && fileId!=null && emailWhoWant != null)
            {
                //Получение id скопированного файла
                string idNewFile = DiskStorage.CopyFile(fileName, fileId, emailWhoWant, diskMainFolder);

                //Запись информации о скопированом файле в БД
                using(UnitOfWork unit = new UnitOfWork())
                {
                    UserFile file = new UserFile() { FileID = idNewFile, FileName = fileName, UserEmail = emailWhoWant };
                    unit.UserFileRepos.AddItem(file);
                    unit.Save();
                }
            }
        }
        #endregion

        #region Создание папок на диске и ПК

        //Создание папки юзера на диске с названием в виде Email пользователя. Либо подгрузка id, если на диске папка есть
        private void CreateFolderByEmailUserInDisk(string email)
        {
            try
            {
                if(email != null && diskMainFolder != null)
                {
                    if(!IsExist(email, diskMainFolder))                     //Есть ли папка пользоваетеля на диске. Если нет, создается
                    {
                        diskFolderCurrentUser = DiskStorage.CreateFolder(email, diskMainFolder);
                    }

                    //Если папка на диске есть, но в программе отсутствует id этой папки, то получем его через метод
                    if(diskFolderCurrentUser == null)
                        diskFolderCurrentUser = DiskStorage.GetIdFile(email, diskMainFolder);
                }
            }
            catch
            {

            }
            
        }

        //Создание папки для файла на ПК
        private bool CreateFolderInPC(string fileName)
        {
            try
            {
                string directoryPath = GetDirectoryPath(fileName);

                if(!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return Directory.Exists(directoryPath);

            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Загрузка на диск, скачивание на ПК

        //Загрузка файла на диск, возвращает id загруженного файла
        private string UploadFileOnDisk(string diskFileName, string fileNamePC, string fileContent, string diskFileFolderId)
        {
            try
            {
                //Перед тем как загрузить, происходит проверка на существование файла с названием загружаемого. Если файл есть, он удалиться
                var idExistFile = DiskStorage.GetIdFile(diskFileName, diskFileFolderId);
                if (idExistFile != null)
                    DiskStorage.DeleteFile(idExistFile);

                return DiskStorage.UploadFile(diskFileName, fileNamePC, fileContent, diskFileFolderId);
            }
            catch
            {
                return null;
            }
        }

        //Скачивание файла на ПК
        private void DownloadFileFromDisk(string diskFileId, string fileNamePC)
        {
            //Перед тем как скачать файл на ПК, проверяется существование файла с таким же названием. Если есть - удаляем
            if (File.Exists(fileNamePC))
                File.Delete(fileNamePC);
            DiskStorage.DownloadFile(diskFileId, fileNamePC);
        }

        #endregion

        #region Функции парса строки с целью нахождения необходимых частей

        //Нахождение имени(не пути!) файла для применения этого имени на диске
        private string GetDiskFileName(string fileName)
        {
            try
            {
                string shortFileName = fileName?.Substring(fileName.LastIndexOf("\\") + 1);

                if (shortFileName == null || shortFileName == "")
                    return null;
                else
                {
                    shortFileName = fileNumber + shortFileName;
                    fileNumber++;

                    return shortFileName;
                }

            }
            catch
            {
                return null;
            }
        }

        //Нахождение расширения файла(jpg, mp3, mp4)
        private string GetFileExtension(string fileName)
        {
            try
            {
                string shortFileName = fileName?.Substring(fileName.LastIndexOf(".") + 1);

                if (shortFileName == null || shortFileName == "")
                    return null;
                else
                    return shortFileName;
            }
            catch
            {
                return null;
            }
        }

        //Из всего имени файла извлекается конечная папка, которая будет содержать файл
        private string GetDirectoryPath(string fileName)
        {
            try
            {
                string directoryPath = fileName?.Substring(0, fileName.LastIndexOf("\\"));

                if (directoryPath == null || directoryPath == "")
                    return null;
                else
                    return directoryPath;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        //Получение contentType файла
        private string GetContentTypeFile(string fileName)
        {
            try
            {
                string extension = GetFileExtension(fileName);
                if (extension != null)
                {
                    switch (extension)
                    {
                        case "jpg": return "image/jpeg";
                        case "mp3": return "audio/mpeg";
                        case "mp4": return "video/mp4";
                        default: return "image/jpeg";
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        //Метод для возврата id папки созданной для текущего юзера. Если папки еще нет, то она создается.
        //Метод, по задумке, должен отработать одним из первых, так как связан с картинкой профиля, а ее загрузка осуществляется при входе в программу
        public string ReturnIdUserDirectory(string email)
        {
            try
            {
                emailCurrentUser = email;
                CreateFolderByEmailUserInDisk(email);
                return diskFolderCurrentUser;
            }
            catch
            {
                return null;
            }
        }

        //Проверка на существование папки на диске для текущего пользователя
        private bool IsExist(string email, string mainFolder)
        {
            return DiskStorage.FolderIsExist(email, mainFolder);
        }

        //Удаление папки пользователя на диске
        public void DeleteFolders(string directoryPath)
        {
            DiskStorage.DeleteFile(directoryPath);
            ToNull();   
        }

        //Метод обнуления полей
        public void ToNull()
        {
            diskFolderCurrentUser = null;
            emailCurrentUser = null;

            fileNumber = 0;
            idFileByFileName = new Dictionary<string, string>();     
        }

        //Создание на диске главной папки, если таковой там нет
        private void CreateIfNotExists()
        {
            //Если самой главной папки на диске нет, значит она создастся
            if (!IsExist(MainFolderKeys.MainStorageDRIVE, "root"))
            {
                diskMainFolder = DiskStorage.CreateFolder(MainFolderKeys.MainStorageDRIVE, "root");
            }
            if (diskMainFolder == null)
                diskMainFolder = DiskStorage.GetIdFile(MainFolderKeys.MainStorageDRIVE, "root");
        }

        //Возврат словаря с добавленными файлами
        public Dictionary<string, string> GetFileCollection()
        {
            return idFileByFileName;
        }

        //Удаление файла с диска
        public void DeleteFileFromDisk(string idFile)
        {
            DiskStorage.DeleteFile(idFile);
        }
    }
}
