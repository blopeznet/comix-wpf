using DirectoryBrowser.Internationalization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryBrowser.ViewModel
{
    partial class MainViewModel
    {

        public void InitSetup()
        {
            //Load values for settings

            _ReaderUsed = new ObservableCollection<string>();
            _ReaderUsed.Add(DirectoryBrowser.Internationalization.Resources.TxtSystem);
            _ReaderUsed.Add(DirectoryBrowser.Internationalization.Resources.TxtApp);

            _Languages = new ObservableCollection<string>();
            _Languages.Add(DirectoryBrowser.Internationalization.Resources.TxtLanguageES);
            _Languages.Add(DirectoryBrowser.Internationalization.Resources.TxtLanguageEN);

            _ThumbnailSources = new ObservableCollection<string>();
            _ThumbnailSources.Add(DirectoryBrowser.Internationalization.Resources.TxtFromFile);
            _ThumbnailSources.Add(DirectoryBrowser.Internationalization.Resources.TxtFromSystem);            
        }

        /// <summary>
        /// Load language start
        /// </summary>
        public void LoadLanguage()
        {

            String IsoLanguage = String.Empty;

            switch (App.ViewModel.LanguageSelected)
            {
                case 0:
                    IsoLanguage = "es-ES";                   
                    break;
                case 1:
                    IsoLanguage = "en-US";
                    CultureResources crusa = new CultureResources();
                    crusa.SetCulture(new CultureInfo(IsoLanguage));
                    break;
            }
            
        }

        /// <summary>
        /// Restart App
        /// </summary>
        public void RestartApp()
        {
            System.Windows.Forms.Application.Restart();
            App.Current.Shutdown();
        }
    

        public void LoadSetup()
        {
            Config.Load();            
            ReaderUsedSelected = Config.Instance.ReaderUsedSelected;
            LanguageSelected = Config.Instance.LanguageSelected;
            ThumbSourceSelected = Config.Instance.ThumbSourceSelected;
        }

        /// <summary>
        /// Save setup values
        /// </summary>
        /// <param name="readerused"></param>
        /// <param name="languageused"></param>
        /// <param name="thumbused"></param>
        public void SaveSetup(int readerused,int languageused,int thumbused)
        {
            Config.Instance.ReaderUsedSelected = readerused;
            Config.Instance.LanguageSelected = languageused;
            Config.Instance.ThumbSourceSelected = thumbused;
            Config.Instance.Save();
        }

        private ObservableCollection<String> _ReaderUsed;
        public ObservableCollection<string> ReaderUsed { get => _ReaderUsed; set => _ReaderUsed = value; }

        private ObservableCollection<String> _Languages;
        public ObservableCollection<string> Languages { get => _Languages; set => _Languages = value; }

        private ObservableCollection<String> _ThumbnailSources;
        public ObservableCollection<string> ThumbnailSources { get => _ThumbnailSources; set => _ThumbnailSources = value; }

        /// <summary>
        /// Selected Index for reader used
        /// </summary>
        private Int32 _ReaderUsedSelected;
        public Int32 ReaderUsedSelected
        {
            get
            {
                return _ReaderUsedSelected;
            }
            set
            {
                _ReaderUsedSelected = value;
                RaisePropertyChanged("ReaderUsedSelected");
            }
        }

        /// <summary>
        /// Selected Index for language used
        /// </summary>
        private Int32 _LanguageSelected;
        public Int32 LanguageSelected
        {
            get
            {
                return _LanguageSelected;
            }
            set
            {
                _LanguageSelected = value;
                RaisePropertyChanged("LanguageSelected");
            }
        }

        /// <summary>
        /// Selected Index for thumbnail source used
        /// </summary>
        private Int32 _ThumbSourceSelected;
        public Int32 ThumbSourceSelected
        {
            get
            {
                return _ThumbSourceSelected;
            }
            set
            {
                _ThumbSourceSelected = value;
                RaisePropertyChanged("ThumbSourceSelected");
            }
        }
    }
}
