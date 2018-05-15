using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace DirectoryBrowser.Views
{
    /// <summary>
    /// View for list files
    /// </summary>
    public partial class FilesView : UserControl
    {
        /// <summary>
        /// Use external app for open file
        /// </summary>
        bool useexternalapp = false;

        public FilesView()
        {
            InitializeComponent();
        }        

        /// <summary>
        /// On mouse push name, display file content with viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String path = ((TextBlock)(e.OriginalSource)).DataContext.ToString();
                if (System.IO.File.Exists(path))
                {                    
                    if (App.ViewModel.ReaderUsedSelected == 0)
                        Process.Start(path);
                    else
                    {
                        App.ViewModel.InitReader(path);
                        ReaderWindow r = new ReaderWindow();
                        r.ShowDialog();
                        
                    }
                }                                   
            }catch(Exception ex) { App.ViewModel.StatusMsg = ex.Message; }
            
        }
    }
}
