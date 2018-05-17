using DirectoryBrowser.Entities;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DirectoryBrowser.ViewModel
{
    /// <summary>
    /// Main Model     
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {

        #region Variables and Properties

        /// <summary>
        /// Timer background worker images
        /// </summary>
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Background images database updating
        /// </summary>
        private readonly BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// Update image comics by count each 10 seconds
        /// </summary>
        private int eachbycomics = 50;

        /// <summary>
        /// Collection treeview populated items from folders
        /// </summary>
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

        /// <summary>
        /// Selected folder for display info
        /// </summary>
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

        /// <summary>
        /// Collection last 10 folders updated from db
        /// </summary>
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

        /// <summary>
        /// Collection all folders from db
        /// </summary>
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
                UpdateSizeStatistics();
                RaisePropertyChanged("AllFolders");
            }
        }

        /// <summary>
        /// Total folders from db size for statistics
        /// </summary>
        private Double _TotalFolderSize;
        public Double TotalFolderSize
        {
            get
            {
                return _TotalFolderSize;
            }
            set
            {
                _TotalFolderSize = value;
                RaisePropertyChanged("TotalFolderSize");
            }
        }

        /// <summary>
        /// Total folders count db for statistics
        /// </summary>
        private Double _TotalFolderCount;
        public Double TotalFolderCount
        {
            get
            {
                return _TotalFolderCount;
            }
            set
            {
                _TotalFolderCount = value;
                RaisePropertyChanged("TotalFolderCount");
            }
        }

        /// <summary>
        /// Total files count db for statistics
        /// </summary>
        private Double _TotalFilesCount;
        public Double TotalFilesCount
        {
            get
            {
                return _TotalFilesCount;
            }
            set
            {
                _TotalFilesCount = value;
                RaisePropertyChanged("TotalFilesCount");
            }
        }

        /// <summary>
        /// Current db filename
        /// </summary>
        private String _CurrentFileNameDB;
        public String CurrentFileNameDB
        {
            get
            {
                return _CurrentFileNameDB;
            }
            set
            {
                _CurrentFileNameDB = value;
                RaisePropertyChanged("CurrentFileNameDB");
            }
        }

        /// <summary>
        /// Copy searched folders from db allfolders (search mode)
        /// </summary>
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

        /// <summary>
        /// Bool flag display is working app
        /// </summary>
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

        /// <summary>
        /// Bool flag display files
        /// </summary>
        private bool _IsShowingFiles;
        public bool IsShowingFiles
        {
            get => _IsShowingFiles;
            set
            {

                _IsShowingFiles = value;
                RaisePropertyChanged("IsShowingFiles");
            }
        }
        
        /// <summary>
        /// String message popup progress
        /// </summary>
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

        /// <summary>
        /// String message status bar left
        /// </summary>
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

        /// <summary>
        /// String message status bar right
        /// </summary>
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

        /// <summary>
        /// Flag show hamburguer lateral menu
        /// </summary>
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

        #endregion


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {           
            ///Instance dispatcher timer for update covers
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();            
        }

        /// <summary>
        /// Dispatcher timer update background images database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UpdateCovers();
        }

        /// <summary>
        /// Method who runs backgroundworker
        /// </summary>
        /// <returns></returns>
        private async Task UpdateCovers()
        {                     
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync();
        }
        
        /// <summary>
        /// Work event for upgrade images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            if (App.ViewModel.AllFolders != null && App.ViewModel.AllFolders.Count > 0)
            {

                int count = App.ViewModel.AllFolders.Count(f => f.CoverExists == false);
                App.ViewModel.StatusMsg = String.Format(DirectoryBrowser.Internationalization.Resources.ThumbsLeftText, count);
                if (count > 0)
                {
                    DBService.Instance.GenerateCoversMemorySteam(App.ViewModel.AllFolders.Where(f => f.CoverExists == false).Take(eachbycomics).OrderBy(f => f.FolderName).ToList(),App.ViewModel.ThumbSourceSelected);
                    if ((count > eachbycomics))
                    App.ViewModel.StatusMsg = String.Format(DirectoryBrowser.Internationalization.Resources.ThumbsLeftText, count-eachbycomics);
                }
                else
                    App.ViewModel.StatusMsg = String.Format(DirectoryBrowser.Internationalization.Resources.AllThumbsText);
            }
        }

        /// <summary>
        /// Method open existing database
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<bool> OpenFileDialog(String path)
        {
            DBService.Instance.Path = path;
            App.ViewModel.FilterMsg = String.Empty;
            App.ViewModel.StatusMsg = String.Empty;
            List<FolderComicsInfo> items = DBService.Instance.GetItemFolders();
            App.ViewModel.AllFolders = items.OrderBy(i => i.FolderName).ToList();
            App.ViewModel.LasFolders = App.ViewModel.AllFolders.OrderByDescending(i => i.CreationDate).Take(10).ToList();
            App.ViewModel.SelectedFolder = App.ViewModel.AllFolders.FirstOrDefault();
            App.ViewModel.sourceCollection = await App.ViewModel.PopulateTreeView(App.ViewModel.AllFolders, '\\');
            App.ViewModel.showMenu = false;
            return true;
        }

        /// <summary>
        /// Method Create new db from folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<bool> OpenFolderDialog(String path)
        {

            String namedbfile =
                String.Format("{0}_{1}.cdb",
                System.IO.Path.GetFileName(path),
                DateTime.Now.ToString("yyyyddMM_HHmm"));


            String filename = UtilsApp.GetDocsPath() + namedbfile;
            App.ViewModel.FilterMsg = String.Empty;
            App.ViewModel.StatusMsg = String.Empty;
            System.IO.Directory.CreateDirectory(UtilsApp.GetDocsPath());
            App.ViewModel.IsWorking = true;
            App.ViewModel.WorkingMsg = DirectoryBrowser.Internationalization.Resources.TxtGenaratingLibraryTitle;
            DBService.Instance.Path = filename;
            List<FolderComicsInfo> items = new List<FolderComicsInfo>();
            items = PShellHelper.Instance.GenerateIndexCollection(path);
            if (items != null && items.Count > 0)
                DBService.Instance.SaveLastFolderCollection(items);
            App.ViewModel.AllFolders = items.OrderBy(f => f.FolderName).ToList();
            App.ViewModel.LasFolders = App.ViewModel.AllFolders.OrderByDescending(i => i.CreationDate).Take(10).ToList();
            App.ViewModel.SelectedFolder = App.ViewModel.AllFolders.FirstOrDefault();
            App.ViewModel.sourceCollection = await App.ViewModel.PopulateTreeView(App.ViewModel.AllFolders, '\\');
            App.ViewModel.IsWorking = false;
            App.ViewModel.WorkingMsg = String.Empty;
            App.ViewModel.showMenu = false;
            return true;

        }

        /// <summary>
        /// Method for populate collection folder info intro Treeview Data
        /// </summary>
        /// <param name="paths">Collecion folder info</param>
        /// <param name="pathSeparator">Character separator folders path</param>
        /// <returns></returns>
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

        /// <summary>
        /// Method close database current
        /// </summary>
        public void CloseCurrentDatabase()
        {
            App.ViewModel.SelectedFolder = null;
            App.ViewModel.AllFolders = null;
            App.ViewModel.LasFolders = null;
            App.ViewModel.SearchedFolders = null;
            App.ViewModel.sourceCollection = null;
            App.ViewModel.StatusMsg = String.Empty;
            App.ViewModel.FilterMsg = String.Empty;
            App.ViewModel.showMenu = true;
        }

        /// <summary>
        /// Update size from folders from database, count files, count folders...
        /// </summary>
        private void UpdateSizeStatistics()
        {
            if (AllFolders != null && AllFolders.Count > 0)
            {

                TotalFolderSize = App.ViewModel.AllFolders.Sum(p => p.TotalSize);
                CurrentFileNameDB = System.IO.Path.GetFileName(DBService.Instance.Path);
                Int32 total = 0;
                foreach (FolderComicsInfo folder in AllFolders)
                {
                    Double percent = (folder.TotalSize * 100 / TotalFolderSize);
                    folder.RelativePercentSize = System.Convert.ToInt32(Math.Round(percent));
                    total+=folder.NumberFiles;
                }

                TotalFolderCount = App.ViewModel.AllFolders.Count;
                TotalFilesCount = total;

            }
        }

        /// <summary>
        /// Method display popup information on MainWindow
        ///</summary>
        /// <param name="msg">String msg to show</param>
        /// <param name="okbutton">String caption button</param>
        public async Task DisplayPopUp(String msg,String okbutton,String id= "RootDialog")
        {
            Views.InfoDialogView view = new Views.InfoDialogView(msg,okbutton);
            view.MinWidth = App.Current.MainWindow.Width / 2;
            view.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            //show the dialog
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(view,id);
        }
    }
}