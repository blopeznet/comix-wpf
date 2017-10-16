/****
 *
 * Source: http://www.codeproject.com/KB/WPF/MarkupExtensionsConverter.aspx
 * 
 ****/
namespace LocalFilesDatabase.FSControls.FileSize.Converter
{
  using System;
  using System.Windows.Data;
  using System.Windows.Markup;

  /// <summary>
  /// Define an enumeration of valid memory units to convert to and from.
  /// </summary>
  public enum MemUnit
  {
    bytes = 0,
    Kb = 1,
    Mb = 2,
    Gb = 3,
    Tb = 4,
    Pb = 5
  }

  /// <summary>
  /// Implement a Converter that converts an input string, such as:
  /// "10000000 bytes"
  /// 
  /// into a human-readable figure, such as: "1.6 Gb"  /// </summary>
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public class NumberToBestSizeConverter : MarkupExtension, IValueConverter
  {
    private static NumberToBestSizeConverter converter;

    public NumberToBestSizeConverter()
    {
    }

    #region IValueConverter Members
    /// <summary>
    /// Helper function to return 32 bit file size indicator in
    /// human readable form (for output as string x bytes, x Kb, x Mb, x Gb)
    /// </summary>
    /// <param name="sizeInBytes"></param>
    /// <param name="sizeString"></param>
    /// <returns></returns>
    public static double GetHumanReadableSize(ulong sizeInBytes, out MemUnit sizeString)
    {
      double dSz = 0;
      sizeString = MemUnit.bytes;

      if (sizeInBytes < 1024)
      {
        sizeString = MemUnit.bytes;
        dSz = sizeInBytes;
      }
      else if (sizeInBytes < (1024 * 512))
      {
        sizeString = MemUnit.Kb;
        dSz = sizeInBytes / 1024;
      }
      else // More than 1 Mb and less than 0,5 Gb ?
        if (sizeInBytes < ((1024.0 * 1024) * 512))
        {
          sizeString = MemUnit.Mb;
          dSz = sizeInBytes / (1024.0 * 1024);
        }
        else if (sizeInBytes < ((1024.0 * 1024 * 1024) * 512))
        {
          sizeString = MemUnit.Gb;
          dSz = sizeInBytes / (1024.0 * 1024 * 1024);
        }
        else if (sizeInBytes < ((1024.0 * 1024 * 1024 * 1024) * 512))
        {
          sizeString = MemUnit.Tb;
          dSz = sizeInBytes / (1024.0 * 1024 * 1024 * 1024);
        }
        else
        {
          sizeString = MemUnit.Pb;
          dSz = sizeInBytes / (1024.0 * 1024 * 1024 * 1024 * 1024);
        }

      return dSz;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new NumberToBestSizeConverter();
      }

      return converter;
    }

    /// <summary>
    /// Convert a bytes size string (eg.: "100 bytes") into a 'meaningful'
    /// size string, such as, (eg.: "0.3 Kbytes") - for display purposes
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
      {
        return Binding.DoNothing;
      }

      string stringInput = value as string;

      if (stringInput != null)
      {
        if (!string.IsNullOrEmpty(value.ToString()))
        {
          ulong numberOfBytes;
          string[] inputComponents;
          if (FileSize.FileSizeContext.ConvertUnparsedSizeToBytesInt(value, out numberOfBytes, out inputComponents) == true)
          {
            MemUnit sizeUnit;
            double doubleVal = GetHumanReadableSize(numberOfBytes, out sizeUnit);
            return string.Format("{0} {1}", Math.Round(doubleVal, 2).ToString(), sizeUnit);
          }
          else
          {
            return string.Format("Syntax Error: '{0}'", stringInput);
          }
        }
      }

      return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }

  /// <summary>
  /// Implement a Converter that converts an array of input strings, such as:
  /// ["10000000 bytes"] or ["10000000", "bytes"]
  /// 
  /// into a human-readable figure, such as: "1.6 Gb"  /// </summary>
  /// </summary>
  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public class NumberToBestSizeConverter2 : MarkupExtension, IMultiValueConverter
  {
    private static NumberToBestSizeConverter2 converter;

    public NumberToBestSizeConverter2()
    {
    }

    #region IValueConverter Members
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new NumberToBestSizeConverter2();
      }

      return converter;
    }

    /// <summary>
    /// Convert a bytes size string (eg.: "100 bytes") into a 'meaningful'
    /// size string, such as, (eg.: "0.3 Kbytes") - for display purposes
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null)
      {
        return Binding.DoNothing;
      }

      string stringInput = string.Empty;

      if (values.Length == 1)
      {
        if (values[0] != null)
          stringInput = string.Format("{0}", values[0]);
      }
      else
      {
        if (values.Length == 2)
        {
          // put value and unit into one string if they were supplied seperately
          if (values[0] != null && values[1] != null)
            stringInput = string.Format("{0} {1}", values[0], values[1]);
        }
      }

      if (stringInput.Length == 0)
      {
        return Binding.DoNothing;
        ////throw new NotImplementedException(string.Format("One or Two input values are required for conversion, but there are; {0}", values.Length));
      }

      ulong numberOfBytes;
      string[] inputComponents;
      if (FileSize.FileSizeContext.ConvertUnparsedSizeToBytesInt(stringInput, out numberOfBytes, out inputComponents) == true)
      {
        ulong[] iMinMax = parameter as ulong[];
        MemUnit sizeUnit;
        string sOutOfRage = string.Empty;

        if (iMinMax != null)
        {
          if (iMinMax.Length == 2)
          {
            ulong iMin = iMinMax[0];
            ulong iMax = iMinMax[1];

            if (numberOfBytes < iMin || numberOfBytes > iMax)
            {
              string sMin, sMax;
              double d = NumberToBestSizeConverter.GetHumanReadableSize(iMin, out sizeUnit);
              sMin = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

              d = NumberToBestSizeConverter.GetHumanReadableSize(iMax, out sizeUnit);
              sMax = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

              sOutOfRage = string.Format(" (Out of range [{0}, {1}])", sMin, sMax);
            }
          }
        }

        double doubleVal = NumberToBestSizeConverter.GetHumanReadableSize(numberOfBytes, out sizeUnit);
        return string.Format("{0} {1}{2}", Math.Round(doubleVal, 2).ToString(), sizeUnit, sOutOfRage);
      }

      return string.Format("Syntax Error: '{0}'", stringInput);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }

  /// <summary>
  /// Attempt to convert a number into an appropriate size statement. The returned string is
  /// empty if the suggested string equals the input string. In other words, a value like:
  /// 
  /// 1> "300 Gb" is converted into (best size) "300 Gb" and therefore the resulting string is empty.
  /// 2> "3000 Gb" is converted into (best size) "2,93 Tb" and therefore the resulting string is "2,93 Tb".
  /// </summary>
  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public class NumberToBestSizeConverter3 : MarkupExtension, IMultiValueConverter
  {
    private static NumberToBestSizeConverter3 converter;

    public NumberToBestSizeConverter3()
    {
    }

    #region IValueConverter Members
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new NumberToBestSizeConverter3();
      }

      return converter;
    }

    /// <summary>
    /// Convert a bytes size string (eg.: "100 bytes") into a 'meaningful'
    /// size string, such as, (eg.: "0.3 Kbytes") - for display purposes
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null)
      {
        return Binding.DoNothing;
      }

      string stringInput = string.Empty;

      if (values.Length == 1)
      {
        if (values[0] != null)
          stringInput = string.Format("{0}", values[0]);
      }
      else
      {
        if (values.Length == 2)
        {
          // put value and unit into one string if they were supplied seperately
          if (values[0] != null && values[1] != null)
            stringInput = string.Format("{0} {1}", values[0], values[1]);
        }
      }

      if (stringInput.Length == 0)
      {
        return Binding.DoNothing;
        ////throw new NotImplementedException(string.Format("One or Two input values are required for conversion, but there are; {0}", values.Length));
      }

      ulong numberOfBytes;
      string[] inputComponents;
      if (FileSize.FileSizeContext.ConvertUnparsedSizeToBytesInt(stringInput, out numberOfBytes, out inputComponents) == true)
      {
        ulong[] iMinMax = parameter as ulong[];
        MemUnit sizeUnit;
        string sOutOfRage = string.Empty;
        bool isOutOfRange = false;

        if (iMinMax != null)
        {
          if (iMinMax.Length == 2)
          {
            ulong iMin = iMinMax[0];
            ulong iMax = iMinMax[1];

            if (numberOfBytes < iMin || numberOfBytes > iMax)
            {
              isOutOfRange = true;
              string sMin, sMax;
              double d = NumberToBestSizeConverter.GetHumanReadableSize(iMin, out sizeUnit);
              sMin = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

              d = NumberToBestSizeConverter.GetHumanReadableSize(iMax, out sizeUnit);
              sMax = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

              sOutOfRage = string.Format(" (Out of range [{0}, {1}])", sMin, sMax);
            }
          }
        }

        double doubleVal = NumberToBestSizeConverter.GetHumanReadableSize(numberOfBytes, out sizeUnit);
        string sOutput = string.Format("{0} {1}", Math.Round(doubleVal, 2).ToString(), sizeUnit);
        string sFinalOutput = string.Format("{0} {1}", sOutput, sOutOfRage);
        string sInput = string.Empty;

        // return ouput only if best size unit does actually differ from input size unit
        if (inputComponents != null)
        {
          if (inputComponents.Length == 2)
          {
            sInput = string.Format("{0} {1}", inputComponents[0], inputComponents[1]);
            
            if (sInput != sOutput)
              return sFinalOutput;
            else
            {
              if (isOutOfRange == false)
                return string.Empty;
              else
                return sFinalOutput;
            }
          }
        }

        // We should never get here but we can return the output even if input components are not available
        return sFinalOutput;
      }

      return string.Format("Syntax Error: '{0}'", stringInput);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
