namespace LocalFilesDatabase.FSControls.FolderPickerLib
{
    using MahApps.Metro.Controls;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;

    /// <summary>
    /// Interaction logic for FolderPickerDialog.xaml
    /// </summary>
    public partial class FolderPickerDialog : MetroWindow
    {
    #region Dependency properties
    public static readonly DependencyProperty ItemContainerStyleProperty =
        DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(FolderPickerDialog));
    #endregion Dependency properties

    #region Constructor
    public FolderPickerDialog()
    {
      this.InitializeComponent();
    }
    #endregion Constructor

    #region Dependency properties
    public Style ItemContainerStyle
    {
      get
      {
        return (Style)GetValue(ItemContainerStyleProperty);
      }

      set
      {
        SetValue(ItemContainerStyleProperty, value);
      }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Get/set selected path to folder that was picked if user OK'ed out of dialog
    /// </summary>
    public string SelectedPath { get; private set; }

    /// <summary>
    /// Set initial path to display in dialog
    /// </summary>
    public string InitialPath
    {
      get
      {
        return this.FolderPickerControl.InitialPath;
      }

      set
      {
        this.FolderPickerControl.InitialPath = value;
      }
    }
    #endregion Properties

    private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as FolderPickerDialog;
      if (control != null)
      {
        control.ItemContainerStyle = e.NewValue as Style;
      }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      if (ComponentDispatcher.IsThreadModal)
      {
        DialogResult = false;
      }
      else
      {
        Close();
      }
    }

    private void CreateButton_Click(object sender, RoutedEventArgs e)
    {
      this.FolderPickerControl.CreateNewFolder();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
      this.FolderPickerControl.RefreshTree();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      // Execute same code as CancelButton_Click
      this.CancelButton_Click(sender, e);
    }

    private void Dialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      // Make sure that input in text box has priority of the currently selected path in treeview
      // (If both differ and user exited with OK)
      if (DialogResult == true)
      {
        // copy selected path from UserControl to Dialog property
        this.SelectedPath = this.FolderPickerControl.SelectedPath;

        if (this.SelectedPath != this.FolderPickerControl.txtPath.Text)
        {
          if (System.IO.Directory.Exists(this.FolderPickerControl.txtPath.Text) == false)
          {
            if (MessageBox.Show(string.Format("The selected path '{0}' does not exist or is not accessible." + System.Environment.NewLine +
                            "Click OK to continue with this path or Cancel to select a different path.", this.FolderPickerControl.txtPath.Text),
                            "Path does not exist",
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            {
              e.Cancel = true;
              return;
            }
          }

          this.SelectedPath = this.FolderPickerControl.txtPath.Text;
        }
        else
        {
          if (System.IO.Directory.Exists(this.SelectedPath) == false)
          {
            if (MessageBox.Show(string.Format("The selected path '{0}' does not exist or is not accessible." + System.Environment.NewLine +
                            "Click OK to continue with this path or Cancel to select a different path.", this.SelectedPath),
                            "Path does not exist",
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            {
              e.Cancel = true;
              return;
            }
          }
        }
      }
    }
  }
}
