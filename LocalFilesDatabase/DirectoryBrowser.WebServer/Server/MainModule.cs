using DirectoryBrowser.WebServer.Entities;
using DirectoryBrowser.WebServerServer;
using Nancy;
using Nancy.Responses;
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

            //View detail
            Get("/views/{uri*}", x => {
                String id = x.uri.ToString().Replace(".html", "");                
                FolderComicsInfo info = Program.ViewModel.GetFolderComicInfoById(id);
                return View["detail.html", info];
            });

            Get("/Search/{term*}", x => {
                String searchby = x.term.ToString().Replace(".html", "");
                var result = Program.ViewModel.Items.Where(f => f.FolderName.Contains(searchby)).ToList();
                return View["index.html",result];
            });

            //Download File
            Get("/comics/{uri*}" , p =>
            {
                dynamic cbrPath = p.uri.ToString();
                bool e = System.IO.File.Exists(cbrPath);

                var file = new FileStream(cbrPath, FileMode.Open);
                String fileName = System.IO.Path.GetFileName(cbrPath);
                StreamResponse response = new StreamResponse(() => file, MimeTypes.GetMimeType(fileName));
                return response.AsAttachment(fileName);                
            });
        }
    }
}
