using System;
using System.Collections.Generic;
using System.Text;
using DirectoryBrowser.WebServer.Entities;
using System.Linq;
using Nancy.Hosting.Self;

namespace DirectoryBrowser.WebServer.ViewModel
{
    public class MainViewModel
    {

        /// <summary>
        /// Collection of FolderComicsInfo get from database
        /// </summary>
        private List<FolderComicsInfo> _Items;
        public List<FolderComicsInfo> Items {
            get { if (_Items == null) { _Items = new List<FolderComicsInfo>(); } return _Items; }
            set => _Items = value;
        }

        /// <summary>
        /// Current Nancy Server
        /// </summary>
        private static NancyHost CurrentNancyHost;

        /// <summary>
        /// Init Server, URL, config, start...
        /// </summary>
        public void InitServer()
        {
            DirectoryBrowser.Host.Utils.AclHelper.AddANewddress("http://+:9664/");
            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;
            CurrentNancyHost = new Nancy.Hosting.Self.NancyHost(new Uri("http://localhost:9664"), new CustomBootstrapper(), hostConfigs);
            CurrentNancyHost.Start();
        }

        /// <summary>
        /// Stop Server
        /// </summary>
        public void StopServer()
        {
            CurrentNancyHost.Stop();
        }

        /// <summary>
        /// Load Folder Comic Info from path database
        /// </summary>
        /// <param name="path"></param>
        public void LoadDataBase(String path= "C:\\Users\\borja\\Documents\\CBDExplorer\\COMICSBL\\Marvel_20180505_0154.cdb")
        {
            
            DBService.Instance.Path = path;
            Items = DBService.Instance.GetItemFolders().OrderBy(i => i.Title).ToList();
            foreach (FolderComicsInfo f in Items)
            {
                //Image 
                f.Base64String = String.Format("data:image/png;base64, {0}", DBService.Instance.LoadFileBase64(f));
                f.CreationDateHtml = TimeFormat(f.CreationDate);
                f.LastUpdateHtml = TimeFormat(f.LastUpdate);
                f.TotalSizeHtml = string.Format("{0:0.00} Mb.", (double)f.TotalSize);


                //Add Info File Detailed
                f.FilesDetailed = new List<FileDetail>();
                foreach (String p in f.Files)
                {
                    FileDetail d = new FileDetail();
                    d.Path = p;
                    d.FileName = System.IO.Path.GetFileNameWithoutExtension(p);
                    f.FilesDetailed.Add(d);
                }
            }

            //Add breaks for display UI
            int maxitemsbyrow = 8;
            int listLength = Items.Count;
            for (int i = 0; i < listLength; i = i + maxitemsbyrow)
            {
                var subitems = Items.Skip(i).Take(maxitemsbyrow);
                subitems.FirstOrDefault().Start = true;
                subitems.LastOrDefault().End = true;
            }

        }

        /// <summary>
        /// Get Folder ComicInfo from Items ViewModel by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FolderComicsInfo GetFolderComicInfoById(String id)
        {
            if (Items == null || Items.Count == 0)
                return null;
            else
            return Items.Where(i => i.Id.ToString() == id).FirstOrDefault();
        }

        /// <summary>
        /// Convert date to text
        /// </summary>
        /// <param name="yourDate"></param>
        /// <returns></returns>
        private string TimeFormat(DateTime yourDate)
        {
            return yourDate.ToShortDateString();
        }

    }
}
