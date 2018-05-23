using DirectoryBrowser.WebServer.Entities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DirectoryBrowser.WebServer
{
    /// <summary>
    /// Service class interaction database
    /// </summary>
    public class DBService
    {
        /// <summary>
        /// Instance service
        /// </summary>
        private static DBService _Instance;
        public static DBService Instance {
            get  {
                if (_Instance == null)
                {
                    _Instance = new DBService();
                    _Instance.Path = "MyComicFiles.cdb";
                }
                return _Instance;
            }
            set => _Instance = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DBService()
        {            
        }

        /// <summary>
        /// Path database
        /// </summary>
        private String _Path;
        public string Path
        {
            get => _Path;
            set
            {
                _Path = value;                
            }
        }    
          
        /// <summary>
        /// Load image from FolderComicsInfo and get Base64
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public String LoadFileBase64(FolderComicsInfo info)
        {
            using (var db = new LiteDatabase(Path))
            {

                LiteFileInfo file = db.FileStorage.FindById(info.Id.ToString());
                if (file != null)
                {
                    try
                    {
                        using (var fStream = file.OpenRead())
                        {
                            var ms = new MemoryStream();
                            fStream.CopyTo(ms);
                            ms.Position = 0;
                            ms.Seek(0, SeekOrigin.Begin);
                            var inputAsString = Convert.ToBase64String(ms.ToArray());
                            return inputAsString;

                        }
                    }
                    catch
                    {
                        return "";
                    }
                }

            }

            return  "";
        }

        /// <summary>
        /// Get collection folders
        /// </summary>
        /// <returns>Directory list</returns>
        public List<FolderComicsInfo> GetItemFolders()
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<FolderComicsInfo>("folders");
                return items.FindAll().ToList();
            }
        }

        /// <summary>
        /// Get Folder by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public FolderComicsInfo GetItemFolder(String Id)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<FolderComicsInfo>("folders");
                return items.FindById(Id);
            }
        }
       
    }
}
