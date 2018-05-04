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

        public string FileNameFirst { get; set; }
        public string FileNameLast { get; set; }

        public string FolderName { get; set; }
        public int NumberFiles { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }

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

        public double TotalSize { get; set; }
        public Int32 RelativePercentSize { get; set; }

        public int Count { get; set; }



        public override string ToString()
        {
            return Path.GetFileName(FolderName);
        }

    }

}
