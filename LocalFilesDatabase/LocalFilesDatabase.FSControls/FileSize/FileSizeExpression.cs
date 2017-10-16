namespace LocalFilesDatabase.FSControls.FileSize
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  internal abstract class FileSizeExpression
  {
    /// <summary>
    /// This property is used to determine there were errors in conversion or not
    /// </summary>
    public bool InterpretOK { get; set; }

    /// <summary>
    /// Abstract Interpret method
    /// </summary>
    /// <param name="value"></param>
    public abstract void Interpret(FileSizeContext value, out string[] strComponents);

    /// <summary>
    /// See also <seealso cref="Interpret"/> method in <seealso cref="TerminalFileSizeExpression"/> class.
    /// </summary>
    /// <param name="value"></param>
    public void InterpretAsBytes(FileSizeContext value)
    {
      string inputNumber = string.Empty;

      try
      {
        string thisPattern = "bytes";

        if (value.Input.EndsWith(thisPattern))
        {
          this.InterpretOK = false;

          try
          {
            // Mask out string portion that is not considered to be a number
            inputNumber = value.Input.Replace(thisPattern, string.Empty);

            // Attempt tp parse the input number portion of the string
            double amount = double.Parse(inputNumber);

            value.Input = string.Format("{0}{1}", amount, thisPattern);

            // Set file size in bytes to output
            value.Output = (ulong)amount;
            this.InterpretOK = true;
          }
          catch (FormatException)
          {
            // These are caught for debugging purposes...
            this.InterpretOK = false;
            value.Output = 0;
            Console.WriteLine("FormatException on '{0}' -> '{1}'", value.Input, inputNumber);
          }
          catch (OverflowException)
          {
            // These are caught for debugging purposes...
            this.InterpretOK = false;
            value.Output = 0;
            Console.WriteLine("OverflowException on '{0}' -> '{1}'", value.Input, inputNumber);
          }
        }
      }
      catch (Exception exp)
      {
        this.InterpretOK = false;
        value.Output = 0;
        Console.WriteLine("Exception on '{0}' -> '{1}'\n{2}", value.Input, inputNumber, exp.ToString());
      }
    }
  }
}
