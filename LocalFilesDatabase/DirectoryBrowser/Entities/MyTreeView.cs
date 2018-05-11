using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace DirectoryBrowser.Entities
{
    /// <summary>
    /// Entity for display treeview
    /// </summary>
    public class MyTreeViewItem:ViewModelBase
    {
        /// <summary>
        /// Level treeview
        /// </summary>
        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// Name to display
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Reference to folder
        /// </summary>
        public FolderComicsInfo Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Show or not content treeview
        /// </summary>
        private bool _Show;
        public bool Show
        {
            get => _Show;
            set
            {

                _Show = value;
                RaisePropertyChanged("Show");
            }
        }

        /// <summary>
        /// Subitems containing for treeview
        /// </summary>
        public List<MyTreeViewItem> SubItems
        {
            get;
            set;
        }
    }

}
