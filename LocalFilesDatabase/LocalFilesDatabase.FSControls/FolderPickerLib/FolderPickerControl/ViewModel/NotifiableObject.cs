namespace LocalFilesDatabase.FSControls.FolderPickerLib
{
  using System;
  using System.ComponentModel;
  using System.Linq.Expressions;

  /// <summary>
  /// Base class for ViewModel classes
  /// </summary>
  public class NotifiableObject : INotifyPropertyChanged
  {
    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;

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
  }
}
