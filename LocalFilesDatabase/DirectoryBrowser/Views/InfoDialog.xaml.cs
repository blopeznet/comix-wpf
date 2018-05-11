using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DirectoryBrowser.Views
{
    /// <summary>
    /// Generic View for display Info
    /// </summary>
    public partial class InfoDialogView : UserControl, INotifyPropertyChanged
    {
        public InfoDialogView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with vars
        /// </summary>
        /// <param name="info">Information text</param>
        /// <param name="oktext">Dismiss text</param>
        public InfoDialogView(String info,string oktext)
        {
            Info = info;
            OkText = oktext;
            InitializeComponent();            
        }    
        
        /// <summary>
        /// Information text
        /// </summary>
        private string _Info = String.Empty;
        public string Info
        {
            get => _Info;
            set
            {
                _Info = value;
                NotifyPropertyChanged("Info");
            }
        }

        /// <summary>
        /// Caption dismiss button
        /// </summary>
        private string _OkText = String.Empty;
        public string OkText
        {
            get => _OkText;
            set
            {
                _OkText = value;
                NotifyPropertyChanged("OkText");
            }
        }

        #region InotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
