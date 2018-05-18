namespace LocalFilesDatabase.FSControls.AutoComplete
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Navigation;

  /// <summary>
  /// Interaction logic for SelectFolderTextBox.xaml
  /// </summary>
  public partial class SelectFolderTextBox : TextBox
  {
    #region fields
    private bool mLoaded = false;
    private string mLastPath;
    private bool mPrevState = false;
    #endregion fields

    #region constructors
    static SelectFolderTextBox()
    {
      // Load deafault style from generic.xaml
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectFolderTextBox),
          new FrameworkPropertyMetadata(typeof(SelectFolderTextBox)));
    }

    public SelectFolderTextBox()
    {
      // this.InitializeComponent();      
    }
    #endregion constructors

    #region properties
    private Popup Popup
    {
      get
      {
        return this.Template.FindName("PART_Popup", this) as Popup;
      }
    }

    private ListBox ItemList
    {
      get { return this.Template.FindName("PART_ItemList", this) as ListBox; }
    }

    private Grid Root
    {
      get { return this.Template.FindName("root", this) as Grid; }
    }

    // 12-25-08 : Add Ghost image when picking from ItemList
    ////TextBlock TempVisual { get { return this.Template.FindName("PART_TempVisual", this) as TextBlock; } }

    private ScrollViewer Host
    {
      get { return this.Template.FindName("PART_ContentHost", this) as ScrollViewer; }
    }

    private UIElement TextBoxView
    {
      get
      {
        foreach (object o in LogicalTreeHelper.GetChildren(this.Host)) return o as UIElement;

        return null;
      }
    }
    #endregion properties

    #region methods
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.mLoaded = true;
      this.KeyDown += new KeyEventHandler(this.AutoCompleteTextBox_KeyDown);
      this.PreviewKeyDown += new KeyEventHandler(this.AutoCompleteTextBox_PreviewKeyDown);
      this.ItemList.PreviewMouseDown += new MouseButtonEventHandler(this.ItemList_PreviewMouseDown);
      this.ItemList.KeyDown += new KeyEventHandler(this.ItemList_KeyDown);

      // TempVisual.MouseDown += new MouseButtonEventHandler(TempVisual_MouseDown);
      // 09-04-09 Based on SilverLaw's approach 
      Popup.CustomPopupPlacementCallback += new CustomPopupPlacementCallback(this.Repositioning);

      Window parentWindow = this.GetParentWindow();
      if (parentWindow != null)
      {
        parentWindow.Deactivated += delegate
        {
          this.mPrevState = this.Popup.IsOpen;
          this.Popup.IsOpen = false;
        };

        parentWindow.Activated += delegate { this.Popup.IsOpen = this.mPrevState; };
      }
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
      if (this.mLoaded)
      {
        try
        {
          ////if (lastPath != Path.GetDirectoryName(this.Text))
          ////if (textBox.Text.EndsWith("\\"))                        
          {
            this.mLastPath = Path.GetDirectoryName(this.Text);
            string[] paths = this.Lookup(this.Text);

            this.ItemList.Items.Clear();
            foreach (string path in paths)
              if (!string.Equals(path, this.Text, StringComparison.CurrentCultureIgnoreCase))
                this.ItemList.Items.Add(path);
          }

          this.Popup.IsOpen = this.ItemList.Items.Count > 0;

          ////ItemList.Items.Filter = p =>
          ////{
          ////    string path = p as string;
          ////    return path.StartsWith(this.Text, StringComparison.CurrentCultureIgnoreCase) &&
          ////        !(String.Equals(path, this.Text, StringComparison.CurrentCultureIgnoreCase));
          ////};
        }
        catch
        {
        }
      }
    }

    private Window GetParentWindow()
    {
      DependencyObject d = this;
      while (d != null && !(d is Window))
        d = LogicalTreeHelper.GetParent(d);
      return d as Window;
    }

    // 09-04-09 Based on SilverLaw's approach 
    private CustomPopupPlacement[] Repositioning(Size popupSize, Size targetSize, Point offset)
    {
      return new CustomPopupPlacement[] {
                new CustomPopupPlacement(new Point((0.01 - offset.X), (this.Root.ActualHeight - offset.Y)), PopupPrimaryAxis.None) };
    }

    private void TempVisual_MouseDown(object sender, MouseButtonEventArgs e)
    {
      string text = Text;
      this.ItemList.SelectedIndex = -1;
      Text = text;
      Popup.IsOpen = false;
    }

    private void AutoCompleteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      // 12-25-08 - added PageDown Support
      if (this.ItemList.Items.Count > 0 && !(e.OriginalSource is ListBoxItem))
      {
        switch (e.Key)
        {
          case Key.Up:
          case Key.Down:
          case Key.Prior:
          case Key.Next:
            this.ItemList.Focus();
            this.ItemList.SelectedIndex = 0;
            ListBoxItem lbi = this.ItemList.ItemContainerGenerator.ContainerFromIndex(this.ItemList.SelectedIndex) as ListBoxItem;
            lbi.Focus();
            e.Handled = true;
            break;
        }
      }
    }

    private void ItemList_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.OriginalSource is ListBoxItem)
      {
        ListBoxItem tb = e.OriginalSource as ListBoxItem;

        e.Handled = true;
        switch (e.Key)
        {
          case Key.Enter:
            this.Text = tb.Content as string;
            this.UpdateSource();
            break;

          // 12-25-08 - added "\" support when picking in list view
          case Key.Oem5:
            this.Text = (tb.Content as string) + "\\";
            break;

          // 12-25-08 - roll back if escape is pressed
          case Key.Escape:
            this.Text = this.mLastPath.TrimEnd('\\') + "\\";
            break;

          default: e.Handled = false;
            break;
        }

        // 12-25-08 - Force focus back the control after selected.
        if (e.Handled)
        {
          Keyboard.Focus(this);
          Popup.IsOpen = false;
          this.Select(Text.Length, 0); // Select last char
        }
      }
    }

    private void AutoCompleteTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        Popup.IsOpen = false;
        this.UpdateSource();
      }
    }

    private void UpdateSource()
    {
      if (this.GetBindingExpression(TextBox.TextProperty) != null)
        this.GetBindingExpression(TextBox.TextProperty).UpdateSource();
    }

    private void ItemList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        TextBlock tb = e.OriginalSource as TextBlock;
        if (tb != null)
        {
          Text = tb.Text;
          this.UpdateSource();
          Popup.IsOpen = false;
          e.Handled = true;
        }
      }
    }

    private string[] Lookup(string path)
    {
      try
      {
        if (Directory.Exists(Path.GetDirectoryName(path)))
        {
          DirectoryInfo lookupFolder = new DirectoryInfo(Path.GetDirectoryName(path));
          if (lookupFolder != null)
          {
            DirectoryInfo[] allItems = lookupFolder.GetDirectories();
            return (from di in allItems where di.FullName.StartsWith(path, StringComparison.CurrentCultureIgnoreCase) select di.FullName).ToArray();
          }
        }
      }
      catch
      {
      }

      return new string[0];
    }
    #endregion methods
  }
}
