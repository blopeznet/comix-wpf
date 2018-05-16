using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DirectoryBrowser.Views
{
    /// <summary>
    /// Generic View for display Info
    /// </summary>
    public partial class SetupDialogView : UserControl, INotifyPropertyChanged
    {
        public SetupDialogView()
        {
            InitializeComponent();
            SaveBackup();
        }

        Config backup;

        private void SaveBackup()
        {
            backup = new Config();
            backup.LanguageSelected = App.ViewModel.LanguageSelected;
            backup.ReaderUsedSelected = App.ViewModel.ReaderUsedSelected;
            backup.ThumbSourceSelected = App.ViewModel.ThumbSourceSelected;
        }

        private void RestoreBackup()
        {            
            App.ViewModel.ReaderUsedSelected = backup.ReaderUsedSelected;
            App.ViewModel.LanguageSelected = backup.LanguageSelected;
            App.ViewModel.ThumbSourceSelected = backup.ThumbSourceSelected;
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

        private void buttonSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.ViewModel.ReaderUsedSelected = cbReader.SelectedIndex;
            App.ViewModel.LanguageSelected = cbLanguage.SelectedIndex;
            App.ViewModel.ThumbSourceSelected = cbThumbnails.SelectedIndex;               
            App.ViewModel.SaveSetup(cbReader.SelectedIndex,cbLanguage.SelectedIndex,cbThumbnails.SelectedIndex);

            App.ViewModel.LoadLanguage();
            App.ViewModel.RestartApp();
        }

        private void buttonCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RestoreBackup();
        }
    }
}
