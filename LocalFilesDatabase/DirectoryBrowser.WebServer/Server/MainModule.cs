using DirectoryBrowser.WebServer.Entities;
using DirectoryBrowser.WebServerServer;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DirectoryBrowser.WebServer
{
	public class MainModule : NancyModule
	{        
        /// <summary>
        /// Definition of routing urls
        /// </summary>
		public MainModule()
		{
            Get("/", x => {               
                return View["index.html",Program.ViewModel.Items];
            });

            Get("/views/{uri*}", x => {
                String id = x.uri.ToString().Replace(".html", "");                
                FolderComicsInfo info = Program.ViewModel.GetFolderComicInfoById(id);
                return View["detail.html", info];
            });
        }
    }
}
