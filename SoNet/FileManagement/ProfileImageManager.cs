using FileManagement.GoogleApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FileManagement
{
    public class ProfileImageManager
    {
        private string contentType = "image/jpeg";                       //Работа будет идти только с картинками расширения *.jpg
        private string image_DRIVE = "profile_pic.jpg";                  //Имя картинки на ДИСКЕ
        private string image_PC;                                         //Имя картинки на ПК
        private string idImage_DRIVE;                                    //Id загруженной на ДИСК картинки

        private static string IdFolderImage_DRIVE;                       //Id папки Profile на ДИСКЕ 
        private string folderImage_PC;                                   //Путь к папке Profile  на ПК

        private string mainFolder_DRIVE;                                 //Id  папки-родителя для папки Profile на ДИСКЕ
        private string mainFolder_PC;                                    //Путь к папке-родителю для папки Profile на ПК

        private string emailCurrentUser;                                 //Email текущего пользователя
        
        private string nameFolderImage_PC_DRIVE;                         //Имя папки для хранения картинки на ДИСКЕ и ПК 


        //Регистрация хранилища для картинок: сохранение email текущего пользователя, создание(если нет) папки на диске этого пользователя, 
        public void RegisterFileStorage(string email, string nameFolderImage = "Profile")
        {
            try
            {
                if (email != null)
                {
                    emailCurrentUser = email;
                    nameFolderImage_PC_DRIVE = nameFolderImage;

                    FileManager fileManager = new FileManager();
                    fileManager.RegisterFileStorage(email);
                    mainFolder_DRIVE = fileManager.ReturnIdUserDirectory(email);

                    mainFolder_PC = "d:/MainFileStorage/" + email;

                    folderImage_PC = mainFolder_PC + "/" + nameFolderImage_PC_DRIVE;
                    image_PC = folderImage_PC + "/" + image_DRIVE;
                }
            }
            catch
            {

            }   
        }


        #region Методы-менеджеры
        //Метод-менеджер по сбору данных и загрузке картинки на диск
        public string ManagementDisk(string fileName)
        {
            try
            {
                IdFolderImage_DRIVE = CreateFolderForProfileImageInDisk(nameFolderImage_PC_DRIVE, mainFolder_DRIVE);      //Создание папки на диске с названием Profile
                //Если создана папка для картинки
                if (IdFolderImage_DRIVE != null)
                {
                    //Проверка на существование картинки на диске 
                    idImage_DRIVE = DiskStorage.GetIdFile(image_DRIVE, IdFolderImage_DRIVE);
                    if (idImage_DRIVE != null)
                        DeleteFileById(idImage_DRIVE);

                    //Загрузка картинки на диск
                    idImage_DRIVE = UploadFileOnDisk(image_DRIVE, fileName, contentType, IdFolderImage_DRIVE);
                    return idImage_DRIVE;
                }

                return null;
            }
            catch
            {
                return null;
            }
           
                
            
        }

        //Метод-менеджер по сбору данных и скачиванию картинки на ПК
        public void ManagementPC(string idImageDISK, string pathImagePC)
        {
            try
            {
                if(DiskStorage.GetIdFile(image_DRIVE, IdFolderImage_DRIVE) != null)
                {
                    CreateFolderForProfileImageInPC(folderImage_PC);                //Создание папки на ПК с названием Profile
                    if (folderImage_PC != null)
                    {
                        if (idImageDISK != null && pathImagePC != null)
                        {
                            //Скачивание картинки на ПК
                            DownloadFileFromDisk(idImageDISK, pathImagePC);
                        }
                    }
                }
            }
            catch
            {

            }
        }


        public string GetFutureImagePath()
        {
            return image_PC;
        }
        #endregion

        #region Создание папок на диске и ПК

        //Создание папки на диске с названием Profile
        private string CreateFolderForProfileImageInDisk(string child, string parent)
        {
            try
            {
                //Проверка на существование папки Profile на Диске
                if (!IsExist(child, parent))
                {
                    //Проверка на наличие Id папки Profile
                    if (IdFolderImage_DRIVE == null)
                        return DiskStorage.CreateFolder(child, parent);
                }

                //Проверка на наличие Id папки Profile
                if (IdFolderImage_DRIVE == null)
                {
                    return DiskStorage.GetIdFile(child, parent);
                }

                return IdFolderImage_DRIVE;
            }
            catch
            {
                return null;
            }   
        }

        //Создание папки на ПК с названием Profile
        private void CreateFolderForProfileImageInPC(string path)
        {
            try
            {
                //Проверка на существование папки Profile на ПК
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Загрузка на диск, скачивание на ПК
        //Загрузка файла на диск, возвращает id загруженного файла. Параметры: как назвать файл на диске, откуда с ПК брать файл, тип файла, в какую папку загрузить
        private string UploadFileOnDisk(string imageNameDRIVE, string imageNamePC, string imageContent, string idFolder)
        {
            return DiskStorage.UploadFile(imageNameDRIVE, imageNamePC, imageContent, idFolder);
        }

        //Скачивание файла на ПК. Параметры: id скачиваемого файла, как назвать на ПК(с учетом всех папок) 
        private void DownloadFileFromDisk(string imageId, string imageNamePC)
        {
            try
            {
                //Проверка на существование файла на ПК, если true, то удаляет файл
                if (File.Exists(imageNamePC))
                    File.Delete(imageNamePC);

                DiskStorage.DownloadFile(imageId, imageNamePC);
            }
            catch
            {

            }
        }

        #endregion

        //Удаление старой картинки на диске перед загрузкой новой
        private void DeleteFileById(string imageId)
        {
            DiskStorage.DeleteFile(imageId);
        }

        //Проверка на существование папки на диске для текущего пользователя
        private bool IsExist(string child, string parent)
        {
            return DiskStorage.FolderIsExist(child, parent);
        }

        public void DeleteFolder()
        {
            DiskStorage.DeleteFile(mainFolder_DRIVE);
            ToNull();
        }

        public void ToNull()
        {
            IdFolderImage_DRIVE = null;
            image_PC = null;
            idImage_DRIVE = null;
            folderImage_PC = null;
            mainFolder_DRIVE = null;
            mainFolder_PC = null;
            nameFolderImage_PC_DRIVE = null;
        }
    }
}
