using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Collections.Generic;
using System.IO;
using System;
using File = Google.Apis.Drive.v3.Data.File;
using System.Threading;
using Google.Apis.Util.Store;

namespace FileManagement.GoogleApi
{
    public class DiskStorage
    {
        private static string[] scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "SoNet";

        private static UserCredential credential;
        private static DriveService service;

        static DiskStorage()
        {
            credential = GetUserCredential();

            service = GetDriveService(credential);
        }

        private static UserCredential GetUserCredential()
        {
            try
            {
                UserCredential credential;

                using(var stream = new FileStream("GoogleApi/client_id.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Path.Combine(Environment.CurrentDirectory, ".credentials", ApplicationName);

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                return credential;
            }
            catch
            {
                return null;
            }

        }

        private static DriveService GetDriveService(UserCredential credential)
        {
            try
            {
                return new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
            }
            catch
            {
                return null;
            }
            
        }

        //Метод загрузки файла на диск
        public static string UploadFile(string diskFileName, string fileNamePC, string fileContent, string diskFileFolderId)
        {
            try
            {
                var fileMetaData = new File();
                fileMetaData.Name = diskFileName;
                fileMetaData.Parents = new List<string> { diskFileFolderId };

                FilesResource.CreateMediaUpload request;

                using (var stream = new FileStream(fileNamePC, FileMode.Open, FileAccess.ReadWrite ,FileShare.Delete | FileShare.ReadWrite))
                {
                    request = service.Files.Create(fileMetaData, stream, fileContent);
                    request.Upload();
                }

                var file = request.ResponseBody;

                return file.Id;
            }
            catch
            {
                return null;
            }
        }

        //Метод выкачивания файла с диска
        public static void DownloadFile(string diskFileId, string fileNamePC)
        {
            try
            {
                var request = service.Files.Get(diskFileId);

                using (var fileStream = new System.IO.FileStream(
                       fileNamePC,
                       System.IO.FileMode.Create,
                       System.IO.FileAccess.Write, FileShare.Delete | FileShare.ReadWrite))
                {
                    request.Download(fileStream);
                }
            }
            catch
            {
                
            }
            
        }

        //Метод создания папки на диске
        public static string CreateFolder(string diskFolderName, string parentDirectory)
        {
            try
            {
                var file = new File();
                file.Name = diskFolderName;
                file.MimeType = "application/vnd.google-apps.folder";
                file.Parents = new List<string> { parentDirectory };

                var request = service.Files.Create(file);
                request.Fields = "id";

                var result = request.Execute();

                return result.Id;
            }
            catch
            {
                return null;
            }
        }

        //Метод копирования файла в рамках диска
        public static string CopyFile(string fileName, string copiedFileId, string emailWhoWantFile, string mainDirectoryDisk)
        {
            try
            {
                var fileMetaData = new File();
                fileMetaData.Name = fileName;

                //Нахождение папки в которую поместим копируемый файл
                string idResultFolder = GetIdFile(emailWhoWantFile, mainDirectoryDisk);
                fileMetaData.Parents = new List<string> { idResultFolder };

                var request = service.Files.Copy(fileMetaData, copiedFileId);

                var result = request.Execute();

                return result.Id;

                
            }
            catch
            {
                return null;
            }
            
        }
        //Проверка на существование определенной папки на диске
        public static bool FolderIsExist(string email, string diskMainFolder)
        {
            try
            {
                var request = service.Files.List();
                request.Q = "'" + diskMainFolder + "' in parents";
                
                var result = request.Execute();

                foreach (var file in result.Files)
                {
                    if (file.Name == email)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //Удаление файла по ID
        public static void DeleteFile(string fileId)
        {
            try
            {
                if(fileId != null)
                {
                    var request = service.Files.Delete(fileId);
                    var result = request.Execute();
                }
            }
            catch
            {

            }
        }

        public static string GetIdFile(string fileName, string diskMainFolder)
        {
            try
            {
                var request = service.Files.List();
                request.Q = "'" + diskMainFolder + "' in parents";

                var result = request.Execute();

                foreach (var file in result.Files)
                {
                    if (file.Name == fileName)
                        return file.Id;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
