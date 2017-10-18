using GalaSoft.MvvmLight;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Collections.Generic;
using LocalFilesDatabase.Entities;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using System.Windows.Controls;

namespace LocalFilesDatabase.ViewModel
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
            SearchModeOn = false;
        }

        /// <summary>
        /// Genera un nuevo Snap
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddNewSnap(String path,String filepath)
        {
            //Create Snap
            WorkingMsg = "OBTENIENDO CARPETAS PARA SNAP...";
            IsWorking = true;
            await Task.Delay(1);
            Snap newsnap = MainUtils.GenerateNewSnap(path,filepath);
            String snapid = newsnap.SnapId;
            //Get Folders
            List<ItemFolder> resultfolders = MainUtils.GenerateSearch(path, snapid);
            //Generate thumbnails  
            WorkingMsg = "GUARDANDO SNAP...";
            DBService.Instance.AddSnap(newsnap);
            int totalfolders = resultfolders.Count-1;
            int processedfolders = 1;
            int totalfiles = 0;
            int processedfiles = 0;
            foreach (ItemFolder f in resultfolders)
            {
                totalfiles = f.Files.Count-1;
                processedfiles = 1;
                foreach (ItemInfo file in f.Files)
                {
                    file.Id = MainUtils.GenerateUniqueIdAsGUID();
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {
                        file.CoverByteArray = MainUtils.BitmapImageToByteArray(MainUtils.CreateThumbnailBitmapImage(file.Path,file));
                    }));
                    processedfiles += 1;                                        
                    WorkingMsg = String.Format("Procesando {0} de {1} carpetas, {2} de {3} ficheros.", processedfolders, totalfolders+1, processedfiles, totalfiles+1);
                }
                DBService.Instance.AddItemFiles(f.Files);
                DBService.Instance.AddItemFolder(f);
                processedfolders += 1;
            }
            
            WorkingMsg = String.Empty;
            IsWorking = false;
            return true;

        }       

        public async Task<bool> LoadData(String path)
        {

            IsWorking = true;
            WorkingMsg = String.Format("CARGANDO MINIATURAS...");
            await Task.Delay(1);
            DBService.Instance.Path = path;
            Files = new List<ItemInfo>();
            Folders = new List<ItemFolder>();
            Snaps = new List<Snap>();
            Snaps = DBService.Instance.GetSnaps();
            if (Snaps.Count > 0)
            {
                SelectedSnap = Snaps[0];
                Folders = DBService.Instance.GetItemFolders(Snaps[0].SnapId);
            }
            if (Folders.Count > 0)
            {
                SelectedFolder = Folders[0];
                Files = DBService.Instance.GetItemFiles(Folders[0].Path);
            }
            if (Files.Count > 0)
            {
                SelectedFile = Files[0];
            }
            RaisePropertyChanged("Files");
            IsDataAvaliable = true;
            RecentFiles = MainUtils.LogRecents(path);
            WorkingMsg = String.Empty;
            IsWorking = false;
            return true;
        }

        public void LoadData()
        {            
            Files = new List<ItemInfo>();
            Folders = new List<ItemFolder>();
            Snaps = new List<Snap>();
            Snaps = DBService.Instance.GetSnaps();
            if (Snaps.Count > 0)
            {
                SelectedSnap = Snaps[0];
                Folders = DBService.Instance.GetItemFolders(Snaps[0].SnapId);
            }
            if (Folders.Count > 0)
            {
                Files = DBService.Instance.GetItemFiles(Folders[0].Path);
            }
            if (Files.Count > 0)
            {
                SelectedFile = Files[0];
            }
            IsDataAvaliable = true;
        }

        public async void OpenFileDialog(String path)
        {
            
            if (File.Exists(path) && !String.IsNullOrEmpty(path))
            {

                IsWorking = true;
                WorkingMsg = "CARGANDO LIBRERIA...";
                await LoadData(path);
                IsDataAvaliable = true;
                WorkingMsg = String.Empty;
                IsWorking = false;
            }
        }
        public async void OpenFileDialog()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Database File (.db)|*.db";
            System.Windows.Forms.DialogResult res = openFileDialog.ShowDialog();
            string path = openFileDialog.FileName;
            if (!String.IsNullOrEmpty(path) && res == System.Windows.Forms.DialogResult.OK)
            {
                await OpenFileLogic(path);
            }
        }

        public async Task<bool> OpenFileLogic(String path)
        {
            IsWorking = true;
            WorkingMsg = "CARGANDO LIBRERIA...";
            await LoadData(path);
            IsDataAvaliable = true;
            WorkingMsg = String.Empty;
            IsWorking = false;
            return true;
        }


        /// <summary>
        /// Reemplazado por OpenFileDialogFolderForSave
        /// </summary>
        public async void OpenFileDialogForSave()
        {            
            System.Windows.Forms.SaveFileDialog dialogfile = new System.Windows.Forms.SaveFileDialog();
            dialogfile.Filter = "Database File (.db)|*.db";
            dialogfile.FileName = DateTime.Now.ToString("yyyyMMdd_hhss") + ".db";
            System.Windows.Forms.DialogResult resultsave = dialogfile.ShowDialog();
            if (resultsave == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();                    
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    IsWorking = true;
                    WorkingMsg = "GENERANDO LIBRERIA...";
                    String path = dialog.SelectedPath;
                    DBService.Instance.Path = dialogfile.FileName;
                    await AddNewSnap(DBService.Instance.Path, dialogfile.FileName);
                    await LoadData(DBService.Instance.Path);
                    IsWorking = false;
                    WorkingMsg = String.Empty;
                }
            }
        }

        public async void OpenFileDialogFolderForSave()
        {

            FSControls.FolderPickerLib.FolderPickerDialog dlg = new FSControls.FolderPickerLib.FolderPickerDialog();
            dlg.InitialPath = "C:";

            if (dlg.ShowDialog() == true)
            {
                App.ViewModel.IsWorking = true;
                String path = dlg.SelectedPath + "\\" + DateTime.Now.ToString("yyyyMMdd_hhss") + ".db";
                FileStream fs = File.Create(path);
                fs.Close();
                WorkingMsg = "GENERANDO LIBRERIA...";                
                DBService.Instance.Path = path;
                await AddNewSnap(dlg.SelectedPath,path);
                await LoadData(DBService.Instance.Path);
                IsWorking = false;
                WorkingMsg = String.Empty;
                App.ViewModel.IsWorking = false;
            }            
        }

        public async void UpdateExistingFolder()
        {
           App.ViewModel.IsWorking = true;
           Snap s = DBService.Instance.GetSnaps()[0];
           String folderpath = s.FolderPath;
           String filepath = s.FilePath;
           s = null;
           File.Delete(filepath);
           CloseDataBase();                      
           
           FileStream fs = File.Create(filepath);
           fs.Close();
           fs = null;
           
           WorkingMsg = "ACTUALIZANDO LIBRERIA LIBRERIA...";
           DBService.Instance.Path = filepath;
           await AddNewSnap(folderpath, filepath);
           await LoadData(DBService.Instance.Path);
           IsWorking = false;
           WorkingMsg = String.Empty;
           App.ViewModel.IsWorking = false;            
        }

        public async Task<bool> SearchCollectionIntoDatabase()
        {
            IsWorking = true;
            await Task.Delay(1);
            WorkingMsg = "BUSCANDO...";            
            Files = new List<ItemInfo>();
            SelectedFile = null;
            if (!String.IsNullOrEmpty(SearchString))
                Folders = DBService.Instance.GetItemFolders(SearchString,true);
            else
                Folders = DBService.Instance.GetItemFolders(SelectedSnap.SnapId);
            if (Folders.Count > 0)
            {
                Files = DBService.Instance.GetItemFiles(Folders[0].Path);
            }
            if (Files.Count > 0)
            {
                SelectedFile = Files[0];
            }
            WorkingMsg = String.Empty;
            IsWorking = false;
            return true;
        }

        public void CloseDataBase()
        {
            SelectedFolder = null;
            Folders = new List<ItemFolder>();
            SelectedFile = null;
            Files = new List<ItemInfo>();
            SelectedSnap = null;
            SearchModeOn = false;
            SearchString = String.Empty;
            IsDataAvaliable = false;
        }

        public void UpdateFiles()
        {
            if (SelectedFolder !=null)
            {                
                Files = DBService.Instance.GetItemFiles(SelectedFolder.Path);
            }
            if (Files.Count > 0)
            {
                SelectedFile = Files[0];
            }
        }

        public async Task<bool> ShowReader(String path,MetroWindow w,int firstpage)
        {
            IsWorking = true;
            WorkingMsg = String.Format("CARGANDO PAGINAS...");
            await Task.Delay(1);
            List<ComicTemp> pages = new List<ComicTemp>();
            App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                pages = MainUtils.CreatePagesComic(path,firstpage);
            }));
            IsWorking = false;
            WorkingMsg = String.Empty;
            await Task.Delay(1);
            ReaderWindow rwindow = new ReaderWindow();            
            rwindow.LoadPages(pages,w,firstpage);
            rwindow.Show();            
            return true;
        }
   
        private bool _IsDataAvaliable;
        public bool IsDataAvaliable
        {
            get => _IsDataAvaliable;
            set
            {
                _IsDataAvaliable = value;
                RaisePropertyChanged("IsDataAvaliable");
            }
        }

        private bool _SearchModeOn;
        public bool SearchModeOn
        {
            get => _SearchModeOn;
            set
            {
                _SearchModeOn = value;
                if (value == false)
                    SearchString = String.Empty;
                RaisePropertyChanged("SearchModeOn");
            }
        }

        private string _SearchString;
        public string SearchString
        {
            get => _SearchString;
            set
            {
                _SearchString = value;
                RaisePropertyChanged("SearchString");
            }
        }

        /// <summary>
        /// Show or hide scrollbar
        /// </summary>
        private ScrollBarVisibility _ShowScrollBar = ScrollBarVisibility.Visible;
        public ScrollBarVisibility ShowScrollBar
        {
            get
            {
                return _ShowScrollBar;
            }

            set
            {
                _ShowScrollBar = value;
                RaisePropertyChanged("ShowScrollBar");
            }
        }

        private bool _usefullscreen = false;
        public bool usefullscreen
        {
            get => _usefullscreen;
            set
            {

                _usefullscreen = value;
                RaisePropertyChanged("usefullscreen");
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

        private Snap _SelectedSnap;
        public Snap SelectedSnap
        {
            get => _SelectedSnap;
            set
            {
                _SelectedSnap = value;
                RaisePropertyChanged("SelectedSnap");
            }
        }

        private Int32 _TotalPagesLoaded;
        public Int32 TotalPagesLoaded
        {
            get => _TotalPagesLoaded;
            set
            {
                _TotalPagesLoaded = value;
                RaisePropertyChanged("TotalPagesLoaded");
            }
        }

        private ItemFolder _SelectedFolder;
        public ItemFolder SelectedFolder {
            get => _SelectedFolder;
            set  {
                _SelectedFolder = value;                
                RaisePropertyChanged("SelectedFolder");
                UpdateFiles();
            }
        }

        private ItemInfo _SelectedFile;
        public ItemInfo SelectedFile
        {
            get => _SelectedFile;
            set
            {
                _SelectedFile = value;
                RaisePropertyChanged("SelectedFile");
            }
        }

        private List<Snap> _Snaps;
        public List<Snap> Snaps
        {
            get => _Snaps;
            set
            {
                _Snaps = value;
                RaisePropertyChanged("Snaps");
            }
        }

        private List<ItemFolder> _Folders;
        public List<ItemFolder> Folders
        {
            get => _Folders;
            set
            {
                _Folders = value;
                RaisePropertyChanged("Folders");
            }
        }

        private List<ItemInfo> _Files;
        public List<ItemInfo> Files
        {
            get => _Files;
            set
            {
                _Files = value;
                RaisePropertyChanged("Files");
            }
        }
        
        private List<String> _RecentFiles;

        public List<string> RecentFiles {
            get
            {
                if (_RecentFiles == null)
                {
                    _RecentFiles = new List<string>();                    
                }
                return _RecentFiles;
            }
            set
                {
                _RecentFiles = value;
                RaisePropertyChanged("RecentFiles");
                }
        }
    }
}