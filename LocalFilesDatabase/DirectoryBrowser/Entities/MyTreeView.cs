using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryBrowser.Entities
{
    public class MyTreeViewItem:ViewModelBase
    {
        public int Level
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public FolderComicsInfo Tag
        {
            get;
            set;
        }

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

        public List<MyTreeViewItem> SubItems
        {
            get;
            set;
        }
    }

}
