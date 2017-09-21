using LiteDB;
using LocalFilesDatabase.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFilesDatabase
{
    /// <summary>
    /// Clase servicio acceso a capa de datos
    /// </summary>
    public class DBService
    {

        private static DBService _Instance;
        public static DBService Instance {
            get  {
                if (_Instance == null)
                {
                    _Instance = new DBService();
                    _Instance.Path = "MyComicFiles.db";
                }
                return _Instance;
            }
            set => _Instance = value;
        }

        public DBService()
        {            
        }

        /// <summary>
        /// Path de la base de datos
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
        /// Añadir Snap a db
        /// </summary>
        /// <param name="newsnap">new snap</param>
        public void AddSnap(Snap newsnap)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<Snap>("snaps");
                items.Insert(newsnap);                
            }
        }

        /// <summary>
        /// Añadir una lista de elementos fichero
        /// </summary>
        /// <param name="newitems">elementos a añadir</param>
        public void AddItemFiles(List<ItemInfo> newitems)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemInfo>("items");
                foreach (ItemInfo f in newitems)
                {
                    var count = items.Count(Query.EQ("Path", f.Path));
                    if (count == 0)
                        items.Insert(f);
                }
            }
        }

        /// <summary>
        /// Añadir una lista de elementos carpeta
        /// </summary>
        /// <param name="newfolders">elementos a añadir</param>
        public void AddItemFolders(List<ItemFolder> newfolders)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemFolder>("folders");
                foreach (ItemFolder f in newfolders)
                {
                    var count = items.Count(Query.EQ("Path", f.Path));
                    if (count == 0)
                        items.Insert(f);
                }
            }
        }

        /// <summary>
        /// Añadir un elemento carpeta
        /// </summary>
        /// <param name="f"></param>
        public void AddItemFolder(ItemFolder f)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemFolder>("folders");                
                var count = items.Count(Query.EQ("Path", f.Path));
                if (count == 0)
                  items.Insert(f);
                
            }
        }

        /// <summary>
        /// Obtener un elemento fichero por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Fichero</returns>
        public ItemInfo GetItemFile(String id)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemInfo>("items");
                ItemInfo item = items.FindById(id);
                return item;
            }
        }

        /// <summary>
        /// Obtener todos los elementos fichero
        /// </summary>
        /// <returns>Lista de ficheros</returns>
        public List<ItemInfo> GetItemFiles()
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemInfo>("items");                
                return items.FindAll().ToList();
            }
        }

        /// <summary>
        /// Obtener un conjunto de elementos fichero filtrado por el path directorio 
        /// </summary>
        /// <param name="SourceFolderPath">Path directorio</param>
        /// <returns>Lista de ficheros</returns>
        public List<ItemInfo> GetItemFiles(string SourceFolderPath)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemInfo>("items");                
                return items.Find(Query.EQ("SourceFolderPath", SourceFolderPath)).ToList();
            }
        }


        /// <summary>
        /// Obtener todos los elementos snap
        /// </summary>        
        /// <returns>Lista de directorios</returns>
        public List<Snap> GetSnaps()
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<Snap>("snaps");
                return items.FindAll().ToList();
            }
        }

        /// <summary>
        /// Obtener todos los elementos carpeta
        /// </summary>        
        /// <returns>Lista de directorios</returns>
        public List<ItemFolder> GetItemFolders()
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemFolder>("folders");
                return items.FindAll().ToList();
            }
        }

        /// <summary>
        /// Obtener un conjunto de elementos fichero filtrado por el path directorio 
        /// </summary>
        /// <param name="SnapId">Id Snap</param>
        /// <returns>Lista de directorios</returns>
        public List<ItemFolder> GetItemFolders(String SnapId)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<ItemFolder>("folders");
                return items.Find(Query.EQ("SnapId", SnapId)).ToList();
            }
        }

        /// <summary>
        /// Obtener un conjunto de elementos fichero filtrado por el path directorio 
        /// </summary>
        /// <param name="SnapId">Id Snap</param>
        /// <returns>Lista de directorios</returns>
        public List<ItemFolder> GetItemFolders(String foldernamefilter,bool aplicarfiltro)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {                
                var items = db.GetCollection<ItemFolder>("folders");
                return items.Find(Query.Contains("Path", foldernamefilter)).ToList();
            }
        }

    }
}
