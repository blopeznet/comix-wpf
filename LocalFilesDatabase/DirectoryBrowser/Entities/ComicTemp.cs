using GalaSoft.MvvmLight;
using SevenZip;
using System.IO;
using System.Windows.Media.Imaging;

namespace DirectoryBrowser.Entities
{
    /// <summary>
    /// Entity Comic Page Internal for viewer
    /// </summary>
    public class ComicTemp : ViewModelBase
    {
        /// <summary>
        /// PageNo
        /// </summary>
        public int NoPage { get; set; }

        /// <summary>
        /// MemoryStream Image
        /// </summary>
        public MemoryStream Source { get; set; }

        /// <summary>
        /// BitmapImage Image
        /// </summary>
        private BitmapImage image;
        public BitmapImage Image { get => image; set { image = value; RaisePropertyChanged("Image"); } }

        /// <summary>
        /// Flag loaded image page
        /// </summary>
        public bool Loaded { get; set; }

        /// <summary>
        /// Reference Archive Info
        /// </summary>
        public ArchiveFileInfo info { get; set; }
    }    
}
