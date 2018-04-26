using DirectoryBrowser.Entities;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Linq;
using System.Windows.Controls;

namespace DirectoryBrowser.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();

            
        }

        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UpdateCovers();
        }

            private async Task UpdateCovers()
        {                     
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync();
        }

        private int eachbycomics = 50;

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            if (App.ViewModel.AllFolders != null && App.ViewModel.AllFolders.Count > 0)
            {

                int count = App.ViewModel.AllFolders.Count(f => f.CoverExists == false);
                App.ViewModel.StatusMsg = String.Format("Quedan {0} miniaturas por generar.",count);
                if (count > 0)
                {
                    DBService.Instance.GenerateCoversMemorySteam(App.ViewModel.AllFolders.Where(f => f.CoverExists == false).Take(eachbycomics).OrderBy(f => f.FolderName).ToList());
                    if ((count > eachbycomics))
                    App.ViewModel.StatusMsg = String.Format("Quedan {0} miniaturas por generar...", count-eachbycomics);
                }
                else
                    App.ViewModel.StatusMsg = String.Format("Todas las miniaturas han sido generadas.");

            }

        }

        public async Task<List<MyTreeViewItem>> PopulateTreeView(IEnumerable<FolderComicsInfo> paths, char pathSeparator)
        {

            List<MyTreeViewItem> sourceCollection = new List<MyTreeViewItem>();
            foreach (FolderComicsInfo path in paths)
            {
                string[] fileItems = path.FolderName.Split(pathSeparator);
                if (fileItems.Any())
                {

                    MyTreeViewItem root = sourceCollection.FirstOrDefault(x => x.Name.Equals(fileItems[0]) && x.Level.Equals(1));
                    if (root == null)
                    {
                        root = new MyTreeViewItem()
                        {
                            Level = 1,
                            Name = fileItems[0],
                            SubItems = new List<MyTreeViewItem>(),
                            Show = true
                        };
                        sourceCollection.Add(root);
                    }

                    if (fileItems.Length > 1)
                    {

                        MyTreeViewItem parentItem = root;
                        int level = 2;
                        for (int i = 1; i < fileItems.Length; ++i)
                        {

                            MyTreeViewItem subItem = parentItem.SubItems.FirstOrDefault(x => x.Name.Equals(fileItems[i]) && x.Level.Equals(level));
                            if (subItem == null)
                            {
                                subItem = new MyTreeViewItem()
                                {
                                    Name = fileItems[i],
                                    Level = level,
                                    SubItems = new List<MyTreeViewItem>(),
                                    Show = true
                                };

                                String subpath = String.Empty;
                                for (int k = 0; k <= i; k++)
                                    subpath += fileItems[k] + "\\";

                                if (subpath == path.FolderName + "\\")
                                {
                                    subItem.Tag = path;
                                    subItem.Show = false;
                                }

                                parentItem.SubItems.Add(subItem);

                            }

                            parentItem = subItem;
                            level++;
                        }
                    }
                }
            }

            return sourceCollection;
        }


        private List<MyTreeViewItem> _sourceCollection;
        public List<MyTreeViewItem> sourceCollection
        {
            get
            {
                if (_sourceCollection == null)
                    _sourceCollection = new List<MyTreeViewItem>();
                return _sourceCollection;
            }

            set
            {
                _sourceCollection = value;
                RaisePropertyChanged("sourceCollection");
            }
        }

        private FolderComicsInfo _SelectedFolder;
        public FolderComicsInfo SelectedFolder
        {
            get
            {                
                return _SelectedFolder;
            }

            set
            {
                _SelectedFolder = value;                
                RaisePropertyChanged("SelectedFolder");
            }
        }

        private List<FolderComicsInfo> _LasFolders;
        public List<FolderComicsInfo> LasFolders
        {
            get
            {
                return _LasFolders;
            }

            set
            {
                _LasFolders = value;
                RaisePropertyChanged("LasFolders");
            }
        }

        private List<FolderComicsInfo> _AllFolders;
        public List<FolderComicsInfo> AllFolders
        {
            get
            {
                return _AllFolders;
            }

            set
            {
                _AllFolders = value;
                RaisePropertyChanged("AllFolders");
            }
        }

        private List<FolderComicsInfo> _SearchedFolders;
        public List<FolderComicsInfo> SearchedFolders
        {
            get
            {
                return _SearchedFolders;
            }

            set
            {
                _SearchedFolders = value;
                RaisePropertyChanged("SearchedFolders");
            }
        }

        private bool _IsWorking;
        public bool IsWorking
        {
            get => _IsWorking;
            set
            {

                _IsWorking = value;
                RaisePropertyChanged("IsWorking");
            }
        }

        private string _WorkingMsg;
        public string WorkingMsg
        {
            get => _WorkingMsg;
            set
            {
                _WorkingMsg = value;
                RaisePropertyChanged("WorkingMsg");
            }
        }

        private string _StatusMsg;
        public string StatusMsg
        {
            get => _StatusMsg;
            set
            {
                _StatusMsg = value;
                RaisePropertyChanged("StatusMsg");
            }
        }

        private string _FilterMsg;
        public string FilterMsg
        {
            get => _FilterMsg;
            set
            {
                _FilterMsg = value;
                RaisePropertyChanged("FilterMsg");
            }
        }



        private bool _showMenu;
        public bool showMenu
        {
            get => _showMenu;
            set
            {

                _showMenu = value;
                RaisePropertyChanged("showMenu");
            }
        }
    }
}