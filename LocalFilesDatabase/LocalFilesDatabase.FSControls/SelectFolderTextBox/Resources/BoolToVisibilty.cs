namespace LocalFilesDatabase.FSControls.AutoComplete.Resources
{
  using System;
  using System.Windows;
  using System.Windows.Data;

  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class BoolToVisibilityConverter : IValueConverter
  {
    private static BoolToVisibilityConverter boolToVisibilityConvInstance = new BoolToVisibilityConverter();

    #region IValueConverter Members
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (!(value is bool))
        return Visibility.Visible;

      if ((bool)value)
        return Visibility.Visible;
      else return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (!(value is Visibility))
        return true;

      return (Visibility)value == Visibility.Visible;
    }

    #endregion
  }
}
