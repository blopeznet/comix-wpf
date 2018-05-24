using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace DirectoryBrowser.WebServer.Entities
{

    /// <summary>
    /// Entity for store into database File Info
    /// </summary>
    public class FileDetail
    {
        public String FileName { get; set; }
        public String Path { get; set; }
        public bool FileExist { get; set; }
    }


    /// <summary>
    /// Entity for store into database Folder Info
    /// </summary>
    public class FolderComicsInfo: INotifyPropertyChanged
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
        /// Files path folder
        /// </summary>
        public List<FileDetail> FilesDetailed { get; set; }

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

        /// <summary>
        /// Title Folder
        /// </summary>
        public string Title { get { return new DirectoryInfo(FolderName).Name; } }

        /// <summary>
        /// Simulate URL for view detail
        /// </summary>
        public string TitleHtml { get { return "/views/" + Id + ".html"; } }

        /// <summary>
        /// Start Row HTML
        /// </summary>
        public bool Start { get; set; }

        /// <summary>
        /// End Row HTML
        /// </summary>
        public bool End { get; set; }

        /// <summary>
        /// Image on Base64 for display
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// Creation date from folder html
        /// </summary>
        public String CreationDateHtml { get; set; }

        /// <summary>
        /// Last update date from folder html
        /// </summary>
        public String LastUpdateHtml { get; set; }

        /// <summary>
        /// Total Size folder html
        /// </summary>
        public String TotalSizeHtml { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }

}
