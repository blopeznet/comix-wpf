using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryBrowser.Utils
{
    public class SevenZipService
    {
        public SevenZipService()
        {
            SevenZipCompressor.SetLibraryPath(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + "\\Libs"),
                    "7z.dll"));
        }

        private static SevenZipService _Instance;

        public static SevenZipService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SevenZipService();
                }
                return _Instance;
            }
            set => _Instance = value;
        }

    }
}
