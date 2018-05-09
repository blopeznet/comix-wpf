using GalaSoft.MvvmLight;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DirectoryBrowser.Entities
{
    public class ComicTemp : ViewModelBase
    {
        public int NoPage { get; set; }
        public MemoryStream Source { get; set; }
        private BitmapImage image;
        public bool Loaded { get; set; }
        public BitmapImage Image { get => image; set { image = value; RaisePropertyChanged("Image"); } }
        public ArchiveFileInfo info { get; set; }
    }    
}
