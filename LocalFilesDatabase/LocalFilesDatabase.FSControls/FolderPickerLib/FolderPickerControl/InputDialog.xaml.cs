namespace LocalFilesDatabase.FSControls.FolderPickerLib
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  
  using FSControls;

  /// <summary>
  /// Interaction logic for InputDialog.xaml
  /// </summary>
  public partial class InputDialog : INotifyPropertyChanged
  {
    private string mMessage;
    private string inputText;

    #region Constructor
    public InputDialog()
    {
      this.InitializeComponent();
    }
    #endregion Constructor

    public new event PropertyChangedEventHandler PropertyChanged;

    #region Properties
    public string Message
    {
      get
      {
        return this.mMessage;
      }

      set
      {
        this.mMessage = value;
        this.OnPropertyChanged("Message");
      }
    }

    public string InputText
    {
      get
      {
        return this.inputText;
      }

      set
      {
        this.inputText = value;
        this.OnPropertyChanged("InputText");
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }
    #endregion Properties

    #region INotifyPropertyChanged Members

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }
}
