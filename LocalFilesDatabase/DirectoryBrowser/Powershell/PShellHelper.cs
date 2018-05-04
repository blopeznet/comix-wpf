using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Collections;
using DirectoryBrowser.Utils;
using LiteDB;
using DirectoryBrowser.Entities;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using DirectoryBrowser;

public class PShellHelper
{

    private static PShellHelper _Instance;

    public static PShellHelper Instance { get { if (_Instance == null) { _Instance = new PShellHelper(); } return _Instance; }  set => _Instance = value; }

    public List<FolderComicsInfo> GenerateIndexCollection(String folderpath)
    {

        try
        {
            String scriptPath = LoadScript();

            PSObject result = null;
            using (PowerShell shell = PowerShell.Create())
            {
                shell.Runspace.SessionStateProxy.PSVariable.Set("root", folderpath);
                Pipeline pipeline = shell.Runspace.CreatePipeline();
                pipeline.Commands.AddScript(scriptPath);
                pipeline.Invoke();
                result = (PSObject)shell.Runspace.SessionStateProxy.PSVariable.GetValue("myArray");
            }


            List<FolderComicsInfo> files = new List<FolderComicsInfo>();
            var baseObj = result.BaseObject;
            if (baseObj is ArrayList)
            {
                var lista = ToList<PSObject>((ArrayList)baseObj);
                foreach (PSObject element in lista)
                {
                    try
                    {
                        String foldername = element.Members["FolderName"].Value.ToString();
                        Int32 NumberFiles = System.Convert.ToInt32(element.Members["Count"].Value.ToString());
                        String filename = element.Members["FileNameFirst"].Value.ToString();
                        String lfilename = element.Members["FileNameLast"].Value.ToString();
                        DateTime date = System.Convert.ToDateTime(element.Members["CreationDate"].Value.ToString());
                        DateTime ldate = System.Convert.ToDateTime(element.Members["LastUpdate"].Value.ToString());
                        String tsize = element.Members["TotalSize"].Value.ToString();

                        List<String> filesl = new List<string>();
                        object a = element.Members["Files"].Value;
                        if (a is PSObject)
                        {
                            filesl = element.Members["Files"].Value.ToString().Split('|').ToList();
                            filesl = filesl.Where(x => !string.IsNullOrEmpty(x)).ToList();
                        }
                        files.Add(
                            new FolderComicsInfo()
                            {
                                CreationDate = date,
                                LastUpdate = ldate,
                                FileNameFirst = filename,
                                FileNameLast = lfilename,
                                NumberFiles = NumberFiles,
                                FolderName = foldername,
                                Count = NumberFiles,
                                TotalSize = System.Convert.ToDouble(tsize),
                                Files = filesl
                            });
                    }
                    catch (Exception ex) {

                    }
                }
            }
            return files;

        }catch(Exception ex)
        {
            App.ViewModel.StatusMsg = "Error al generar el indice, revise el nombre de los directorios.";
            return new List<FolderComicsInfo>();
        }
    }

    /// <summary>
    /// Convert ArrayList to List.
    /// </summary>
    private List<T> ToList<T>( ArrayList arrayList)
    {
        List<T> list = new List<T>(arrayList.Count);
        foreach (T instance in arrayList)
        {
            list.Add(instance);
        }
        return list;
    }    

    // helper method that takes your script path, loads up the script 
    // into a variable, and passes the variable to the RunScript method 
    // that will then execute the contents 
    private string LoadScript()
    {
        try
        {

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DirectoryBrowser.Assets.Scripts.list.ps1");
            // Create an instance of StreamReader to read from our file. 
            // The using statement also closes the StreamReader. 
            using (StreamReader sr = new StreamReader(stream))
            {

                // use a string builder to get all our lines from the file 
                StringBuilder fileContents = new StringBuilder();

                // string to hold the current line 
                string curLine;

                // loop through our file and read each line into our 
                // stringbuilder as we go along 
                while ((curLine = sr.ReadLine()) != null)
                {
                    // read each line and MAKE SURE YOU ADD BACK THE 
                    // LINEFEED THAT IT THE ReadLine() METHOD STRIPS OFF 
                    fileContents.Append(curLine + "\n");
                }

                // call RunScript and pass in our file contents 
                // converted to a string 
                return fileContents.ToString();
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong. 
            string errorText = "The file could not be read:";
            errorText += e.Message + "\n";
            return errorText;
        }

    }

    
}


