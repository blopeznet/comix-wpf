using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryBrowser.ViewModel
{
    partial class MainViewModel
    {

        private void InitSetup()
        {
            _ReaderUsed = new ObservableCollection<string>();
            _ReaderUsed.Add("Sistema");
            _ReaderUsed.Add("Aplicación");

            _Languages = new ObservableCollection<string>();
            _Languages.Add("Español");
            _Languages.Add("Inglés");

            _ThumbnailSources = new ObservableCollection<string>();
            _ThumbnailSources.Add("Desde fichero");
            _ThumbnailSources.Add("Desde sistema");

            ReaderUsedSelected = 1;
            LanguageSelected = 0;
            _ThumbSourceSelected = 1;
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
