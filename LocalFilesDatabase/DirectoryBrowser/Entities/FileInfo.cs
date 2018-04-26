using GalaSoft.MvvmLight;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryBrowser.Entities
{
    public class FolderComicsInfo:ViewModelBase
    {
        [BsonId]
        public Guid Id { get; set; }

        public string FileName { get; set; }
        public string FolderName { get; set; }
        public int NumberFiles { get; set; }
        public DateTime Date { get; set; }

        public List<String> Files { get; set; }

        private bool _CoverExists;
        public bool CoverExists
        {
            get => _CoverExists;
            set
            {
                _CoverExists = value;
                RaisePropertyChanged("CoverExists");
            }
        }


        public override string ToString()
        {
            return Path.GetFileName(FolderName);
        }

    }

}
