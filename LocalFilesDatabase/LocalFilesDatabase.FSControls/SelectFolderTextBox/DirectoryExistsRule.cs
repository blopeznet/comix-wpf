namespace LocalFilesDatabase.FSControls.AutoComplete
{
  using System;
  using System.IO;
  using System.Windows.Controls;

  public class DirectoryExistsRule : ValidationRule
  {
    private static DirectoryExistsRule directoryExistsRuleInstance = new DirectoryExistsRule();

    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
      try
      {
        if (!(value is string))
          return new ValidationResult(false, "InvalidPath");

        if (!Directory.Exists((string)value))
          return new ValidationResult(false, "Path Not Found");
      }
      catch
      {
        return new ValidationResult(false, "Invalid Path");
      }

      return new ValidationResult(true, null);
    }
  }
}
