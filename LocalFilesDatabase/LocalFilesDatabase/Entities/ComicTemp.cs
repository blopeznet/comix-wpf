using GalaSoft.MvvmLight;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LocalFilesDatabase.Entities
{
    public class ComicTemp : ViewModelBase
    {
        public String Source { get; set; }
        private BitmapImage image;
        public bool Loaded { get; set; }
        public BitmapImage Image { get => image; set { image = value; RaisePropertyChanged("Image"); } }
    }    
}
