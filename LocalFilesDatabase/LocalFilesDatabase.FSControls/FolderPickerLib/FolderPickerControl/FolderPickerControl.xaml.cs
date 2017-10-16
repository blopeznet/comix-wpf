namespace LocalFilesDatabase.FSControls.FolderPickerLib
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;

  /// <summary>
  /// Interaction logic for FolderPicker.xaml
  /// </summary>
  public partial class FolderPickerControl : UserControl, INotifyPropertyChanged
  {
    #region Constants
    private const string EmptyItemName = "Empty";
    private const string NewFolderName = "New Folder";
    private const int MaxNewFolderSuffix = 10000;
    #endregion

    #region Private fields
    private TreeItem root;
    private TreeItem mSelectedItem;
    private string mInitialPath;
    private string mSelectedPath = string.Empty;
    private Style mItemContainerStyle;
    #endregion

    #region Constructor
    public FolderPickerControl()
    {
      this.InitializeComponent();

      this.Init();
    }
    #endregion Constructor

    public event PropertyChangedEventHandler PropertyChanged;

    #region Properties
    /// <summary>
    /// Get/set root item of treeview
    /// </summary>
    public TreeItem Root
    {
      get
      {
        return this.root;
      }

      private set
      {
        // for some (unknown) reason this cannot be guarded with an if != value block ???
        this.root = value;
        this.NotifyPropertyChanged(() => this.Root);
      }
    }

    /// <summary>
    /// Get/set currently selected item in the treeview
    /// </summary>
    public TreeItem SelectedItem
    {
      get
      {
        return this.mSelectedItem;
      }

      private set
      {
        if (this.mSelectedItem != value)
        {
          this.mSelectedItem = value;

          if (value != null)
            this.SelectedPath = this.mSelectedItem.GetFullPath();
          else
            this.SelectedPath = null;

          this.NotifyPropertyChanged(() => this.SelectedItem);
        }
      }
    }

    /// <summary>
    /// Get/set path of the currently selected element in the treeview
    /// </summary>
    public string SelectedPath
    {
      get
      {
        return this.mSelectedPath;
      }

      // Setter is called by TreeView_SelectedItem event ONLY
      // It is currently not clear how to sync back new paths to the tree view - although its possible through the
      // UpdateInitialPathUI method but the textbox looses its focus if that method is used - so a method without loosing
      // focus would be preferable ...
      private set
      {
        if (value != this.mSelectedPath)
        {
          this.mSelectedPath = value;

          // Sync back to treeView Problem: TreeView acquires focus and textbox cannot be used for input
          ////if (System.IO.Directory.Exists(this.mSelectedPath))
          ////{
            ////if (this.SelectedItem != null)
            ////{
              ////if (this.mSelectedPath != this.SelectedItem.GetFullPath())
              ////{
              ////  this.InitialPath = this.mSelectedPath;
              ////}
            ////}
            ////else
            ////{
            ////  this.InitialPath = this.mSelectedPath;
            ////}
          ////}

          this.NotifyPropertyChanged(() => this.SelectedPath);
        }
        ////this.txtPath.Text = value;  // DBa set path into path text box
      }
    }

    /// <summary>
    /// Get/set initial path to be displayed in treeview
    /// </summary>
    public string InitialPath
    {
      get
      {
        return this.mInitialPath;
      }

      set
      {
        if (this.mInitialPath != value)
        {
          this.mInitialPath = value;
          this.UpdateInitialPathUI();
        }
      }
    }

    public Style ItemContainerStyle
    {
      get
      {
        return this.mItemContainerStyle;
      }

      set
      {
        if (this.mItemContainerStyle != value)
        {
          this.mItemContainerStyle = value;
          this.OnPropertyChanged("ItemContainerStyle");
        }
      }
    }
    #endregion

    public void CreateNewFolder()
    {
      this.CreateNewFolderImpl(this.SelectedItem);
    }

    public void RefreshTree()
    {
      this.Root = null;
      this.Init();

      this.UpdateInitialPathUI();
    }

    #region INotifyPropertyChanged Members
    public void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
    {
      var lambda = (LambdaExpression)property;
      MemberExpression memberExpression;
      if (lambda.Body is UnaryExpression)
      {
        var unaryExpression = (UnaryExpression)lambda.Body;
        memberExpression = (MemberExpression)unaryExpression.Operand;
      }
      else memberExpression = (MemberExpression)lambda.Body;

      this.OnPropertyChanged(memberExpression.Member.Name);
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion

    #region Private methods
    private void Init()
    {
      this.root = new TreeItem("root", null);
      var systemDrives = DriveInfo.GetDrives();

      foreach (var sd in systemDrives)
      {
        var item = new DriveTreeItem(sd.Name, sd.DriveType, this.root);
        item.Children.Add(new TreeItem(EmptyItemName, item));

        this.root.Children.Add(item);
      }

      this.Root = this.root; // to notify UI
    }

    private void TreeView_Selected(object sender, RoutedEventArgs e)
    {
      var tvi = e.OriginalSource as TreeViewItem;

      if (tvi != null)
        this.SelectedItem = tvi.DataContext as TreeItem;
    }

    private void TreeView_Expanded(object sender, RoutedEventArgs e)
    {
      var tvi = e.OriginalSource as TreeViewItem;
      var treeItem = tvi.DataContext as TreeItem;

      if (treeItem != null)
      {
        if (!treeItem.IsFullyLoaded)
        {
          treeItem.Children.Clear();

          string path = treeItem.GetFullPath();

          DirectoryInfo dir = new DirectoryInfo(path);

          try
          {
            var subDirs = dir.GetDirectories();
            foreach (var sd in subDirs)
            {
              TreeItem item = new TreeItem(sd.Name, treeItem);
              item.Children.Add(new TreeItem(EmptyItemName, item));

              treeItem.Children.Add(item);
            }
          }
          catch
          {
          }

          treeItem.IsFullyLoaded = true;
        }
      }
      else
        throw new Exception();
    }

    private void UpdateInitialPathUI()
    {
      // Always show path as selected path even if we find out below that:
      // 1> It does not exist
      // 2> Or cannot be rendered in the control
      this.SelectedPath = this.InitialPath;

      ////if (!Directory.Exists(this.InitialPath))
      ////  return;

      var initialDir = new DirectoryInfo(this.InitialPath);

      ////if (!initialDir.Exists)
      ////  return;

      // Get stack of directory infos (one object for each element from root to sub-dir)
      Stack<DirectoryInfo> stack = this.TraverseUpToRoot(initialDir);
      var containerGenerator = this.TreeView.ItemContainerGenerator;
      var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
      DirectoryInfo currentDir = null;
      var dirContainer = this.Root;

      AutoResetEvent waitEvent = new AutoResetEvent(true);

      if (stack != null)
      {
        Task processStackTask = Task.Factory.StartNew(() =>
        {
          while (stack.Count > 0)
          {
            waitEvent.WaitOne();

            currentDir = stack.Pop();

            Task waitGeneratorTask = Task.Factory.StartNew(() =>
            {
              if (containerGenerator == null)
                return;

              while (containerGenerator.Status != GeneratorStatus.ContainersGenerated)
                Thread.Sleep(50);
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            Task updateUiTask = waitGeneratorTask.ContinueWith((r) =>
            {
              try
              {
                var childItem = dirContainer.Children.Where(c => c.Name == currentDir.Name).FirstOrDefault();
                var tvi = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                dirContainer = tvi.DataContext as TreeItem;
                tvi.IsExpanded = true;

                tvi.Focus();

                containerGenerator = tvi.ItemContainerGenerator;
              }
              catch
              {
              }

              waitEvent.Set();
            }, uiContext);
          }
        }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
      }
    }

    /// <summary>
    /// Build a stack (queue) of directory entries from root to subDir and return it
    /// </summary>
    /// <param name="subDir"></param>
    /// <returns></returns>
    private Stack<DirectoryInfo> TraverseUpToRoot(DirectoryInfo subDir)
    {
      if (subDir == null)
        return null;

      if (!subDir.Exists)
        return null;

      Stack<DirectoryInfo> queue = new Stack<DirectoryInfo>();
      queue.Push(subDir);
      DirectoryInfo ti = subDir.Parent;

      while (ti != null)
      {
        queue.Push(ti);
        ti = ti.Parent;
      }

      return queue;
    }

    private void CreateNewFolderImpl(TreeItem parent)
    {
      try
      {
        if (parent == null)
          return;

        var parentPath = parent.GetFullPath();
        var newDirName = this.GenerateNewFolderName(parentPath);
        var newPath = Path.Combine(parentPath, newDirName);

        Directory.CreateDirectory(newPath);

        var childs = parent.Children;
        var newChild = new TreeItem(newDirName, parent);
        childs.Add(newChild);
        parent.Children = childs.OrderBy(c => c.Name).ToObservableCollection();
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Can't create new folder. Error: {0}", ex.Message));
      }
    }

    private string GenerateNewFolderName(string parentPath)
    {
      string result = NewFolderName;

      if (Directory.Exists(Path.Combine(parentPath, result)))
      {
        for (int i = 1; i < MaxNewFolderSuffix; ++i)
        {
          var nameWithIndex = string.Format(NewFolderName + " {0}", i);

          if (!Directory.Exists(Path.Combine(parentPath, nameWithIndex)))
          {
            result = nameWithIndex;
            break;
          }
        }
      }

      return result;
    }

    private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
    {
      var item = sender as MenuItem;
      if (item != null)
      {
        var context = item.DataContext as TreeItem;
        this.CreateNewFolderImpl(context);
      }
    }

    private void RenameMenuItem_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var item = sender as MenuItem;
        if (item != null)
        {
          var context = item.DataContext as TreeItem;
          if (context != null && !(context is DriveTreeItem))
          {
            var dialog = new InputDialog()
            {
              Message = "New folder name:",
              InputText = context.Name,
              Title = string.Format("Do you really want to rename folder {0}?", context.Name)
            };

            if (dialog.ShowDialog() == true)
            {
              var newFolderName = dialog.InputText;

              /*
               * Parent for context is always not null due to the fact
               * that we don't allow to change the name of DriveTreeItem
               */
              var newFolderFullPath = Path.Combine(context.Parent.GetFullPath(), newFolderName);
              if (Directory.Exists(newFolderFullPath))
              {
                MessageBox.Show(string.Format("Directory already exists: {0}", newFolderFullPath));
              }
              else
              {
                Directory.Move(context.GetFullPath(), newFolderFullPath);
                context.Name = newFolderName;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Can't rename folder. Error: {0}", ex.Message));
      }
    }

    private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var item = sender as MenuItem;
        if (item != null)
        {
          var context = item.DataContext as TreeItem;
          if (context != null && !(context is DriveTreeItem))
          {
            var confirmed =
                MessageBox.Show(
                    string.Format("Do you really want to delete folder {0}?", context.Name),
                    "Confirm folder removal",
                    MessageBoxButton.YesNo);

            if (confirmed == MessageBoxResult.Yes)
            {
              Directory.Delete(context.GetFullPath());
              var parent = context.Parent;
              parent.Children.Remove(context);
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Can't delete folder. Error: {0}", ex.Message));
      }
    }

    #endregion
  }

  public class NullToBoolConverter : IValueConverter
  {
    #region IValueConverter Members
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return false;

      return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
