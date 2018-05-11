using GalaSoft.MvvmLight;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;

namespace DirectoryBrowser.Entities
{
    /// <summary>
    /// Entity for store into database Folder Info
    /// </summary>
    public class FolderComicsInfo:ViewModelBase
    {

        [BsonId]
        public Guid Id { get; set; }

        /// <summary>
        /// Firs file by creation date into folder
        /// </summary>
        public string FileNameFirst { get; set; }
        public string FileNameLast { get; set; }

        /// <summary>
        /// Folder path
        /// </summary>
        public string FolderName { get; set; }
        public int NumberFiles { get; set; }

        /// <summary>
        /// Creation date from folder
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Last update date from folder
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Files path folder
        /// </summary>
        public List<String> Files { get; set; }

        /// <summary>
        /// Flag cover exists
        /// </summary>
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

        /// <summary>
        /// Total Size folder
        /// </summary>
        public double TotalSize { get; set; }

        /// <summary>
        /// Relative percent size
        /// </summary>
        public Int32 RelativePercentSize { get; set; }

        /// <summary>
        /// Number files containing folder
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Override toString to print FolderName
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Path.GetFileName(FolderName);
        }

    }

}
