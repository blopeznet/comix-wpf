using GalaSoft.MvvmLight;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFilesDatabase.Entities
{
    public enum ItemType
    {
      Empty,
      Folder,
      CBR,
      CBZ,
      PDF
    }

    public class Snap : ViewModelBase
    {
        [BsonId]
        public Guid Id { get; set; }

        private String _SnapId;
        public string SnapId
        {
            get => _SnapId;
            set
            {
                _SnapId = value;
                RaisePropertyChanged("SnapId");
            }
        }

        private String _Description;
        public string Description
        {
            get => _Description;
            set
            {
                _Description = value;
                RaisePropertyChanged("Description");
            }
        }

        private String _FolderPath;
        public string FolderPath
        {
            get => _FolderPath;
            set
            {
                _FolderPath = value;
                RaisePropertyChanged("FolderPath");
            }
        }

        private String _FilePath;
        public string FilePath
        {
            get => _FilePath;
            set
            {
                _FilePath = value;
                RaisePropertyChanged("FilePath");
            }
        }


        private DateTime _CreationDate;
        public DateTime CreationDate
        {
            get => _CreationDate;
            set
            {
                _CreationDate = value;
                RaisePropertyChanged("CreationDate");
            }
        }

        private DateTime _LastUpdateDate;
        public DateTime LastUpdateDate
        {
            get => _LastUpdateDate;
            set
            {
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }

        private Double _Size;
        public Double Size
        {
            get => _Size;
            set
            {
                _Size = value;
                RaisePropertyChanged("Size");
            }
        }


        public virtual List<ItemFolder> ItemList
        { get; set; }
    }

    public class ItemFolder :ViewModelBase
    {
        [BsonId]
        public Guid Id { get; set; }

        private String _SnapId;
        public string SnapId
        {
            get => _SnapId;
            set
            {
                _SnapId = value;
                RaisePropertyChanged("SnapId");
            }
        }

        private String _Path;
        public string Path
        {
            get => _Path;
            set
            {
                _Path = value;
                RaisePropertyChanged("Path");
            }
        }

        private int _Level;
        public int Level
        {
            get => _Level;
            set
            {
                _Level = value;
                RaisePropertyChanged("Level");
            }
        }

        private int _FilesCount;
        public int FilesCount
        {
            get => _FilesCount;
            set
            {
                _FilesCount = value;
                RaisePropertyChanged("FilesCount");
            }
        }

        private DateTime _CreationDate;
        public DateTime CreationDate
        {
            get => _CreationDate;
            set
            {
                _CreationDate = value;
                RaisePropertyChanged("CreationDate");
            }
        }

        private DateTime _LastUpdateDate;
        public DateTime LastUpdateDate
        {
            get => _LastUpdateDate;
            set
            {
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }

        private Double _Size;
        public Double Size
        {
            get => _Size;
            set
            {
                _Size = value;
                RaisePropertyChanged("Size");
            }
        }

        private List<ItemInfo> _Files;
        public virtual List<ItemInfo> Files {
            get { if (_Files==null) _Files = new List<ItemInfo>();return _Files; }
            set => _Files = value;
        }
        
        public override string ToString()
        {
            return String.Format("{0}",Path);
        }
    }

    public class ItemInfo :ViewModelBase
    {
        [BsonId]
        public Guid Id { get; set; }

        private String _SnapId;
        public string SnapId
        {
            get => _SnapId;
            set
            {
                _SnapId = value;
                RaisePropertyChanged("SnapId");
            }
        }

        private String _Path;
        public string Path
        {
            get => _Path;
            set
            {
                _Path = value;
                RaisePropertyChanged("Path");
            }
        }

        private String _DisplayName;
        public string DisplayName
        {
            get => _DisplayName;
            set
            {
                _DisplayName = value;
                RaisePropertyChanged("DisplayName");
            }
        }

        private String _SourceFolderPath;
        public string SourceFolderPath
        {
            get => _SourceFolderPath;
            set
            {
                _SourceFolderPath = value;
                RaisePropertyChanged("SourceFolderPath");
            }
        }

        private byte[] _CoverByteArray;
        public byte[] CoverByteArray
        {
            get => _CoverByteArray;
            set
            {
                _CoverByteArray = value;
                RaisePropertyChanged("CoverByteArray");
            }
        }

        private ItemType _Type;
        public ItemType Type
        {
            get => _Type;
            set
            {
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }

        private DateTime _CreationDate;
        public DateTime CreationDate
        {
            get => _CreationDate;
            set
            {
                _CreationDate = value;
                RaisePropertyChanged("CreationDate");
            }
        }

        private Double _Size;
        public Double Size
        {
            get => _Size;
            set
            {
                _Size = value;
                RaisePropertyChanged("Size");
            }
        }

        private Int32 _CurrentPages;
        public Int32 CurrentPages
        {
            get => _CurrentPages;
            set
            {
                _CurrentPages = value;
                RaisePropertyChanged("CurrentPages");
            }
        }

        private Int32 _TotalPages;
        public Int32 TotalPages
        {
            get => _TotalPages;
            set
            {
                _TotalPages = value;
                RaisePropertyChanged("TotalPages");
            }
        }


    }
}
