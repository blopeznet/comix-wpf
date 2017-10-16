namespace LocalFilesDatabase.FSControls.FileSize
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  internal abstract class TerminalFileSizeExpression : FileSizeExpression
  {
    /// <summary>
    /// Interpret input string and set corresponding <seealso cref="InterpretOK"/> property
    /// 
    /// See also <seealso cref="InterpretAsBytes"/> method in <seealso cref="FileSizeExpression"/> class.
    /// </summary>
    /// <param name="value"></param>
    public override void Interpret(FileSizeContext value,
                                   out string[] strComponents)
    {
      strComponents = null;
      string inputNumber = string.Empty;

      try
      {
        string thisPattern = this.ThisPattern();

        if (value.Input.EndsWith(thisPattern))
        {
          this.InterpretOK = false;

          try
          {
            // Mask out string portion that is not considered to be a number
            inputNumber = value.Input.Replace(this.ThisPattern(), string.Empty);

            // Attempt tp parse the input number portion of the string as double
            double amount = double.Parse(inputNumber);

            // Compute file size in bytes
            ulong fileSize = (ulong)(amount * 1024);

            value.Input = string.Format("{0}{1}", fileSize, this.NextPattern());

            // Set file size in bytes to output
            value.Output = fileSize;

            strComponents = new string[2];         // Pass back portions of recognized string
            strComponents[0] = inputNumber.Trim();
            strComponents[1] = this.ThisPattern();

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

    protected abstract string ThisPattern();

    protected abstract string NextPattern();
  }
}
