using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DirectoryBrowser
{
    public class Config
    {
        /// <summary>
        /// 
        /// </summary>
        public int ReaderUsedSelected;

        /// <summary>
        /// 
        /// </summary>
        public int LanguageSelected;

        /// <summary>
        /// 
        /// </summary>
        public int ThumbSourceSelected;

        // Sets the default values of all configurable values.
        private void SetDefaultValues()
        {
            this.ReaderUsedSelected = 1;
            this.LanguageSelected = 0;
            this.ThumbSourceSelected = 1;
        }

        // ---------------------------------------------------------------------------------------

        // Name of configuration file.
        [XmlIgnore]
        public static string FileName = "config.xml";

        // Globally accessable instance of loaded configuration.        
        private static Config _Instance;
        [XmlIgnore]
        public static Config Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Config();
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }

        // Empty constructor for XmlSerializer.
        public Config()
        {
        }

        // Used to load the default configuration if Load() fails.
        public static void Default()
        {            
            Config.Instance.SetDefaultValues();
        }

        // Loads the configuration from file.
        public static bool Load()
        {

            if (System.IO.File.Exists(Config.FileName))
            {
                var serializer = new XmlSerializer(typeof(Config));                
                using (var fStream = new FileStream(Config.FileName, FileMode.OpenOrCreate))
                {
                    Config.Instance = (Config)serializer.Deserialize(fStream);
                }

                return true;
            }
            else
            {
                
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(Config.FileName)))                
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Config.FileName));
                

                Default();
                var serializer = new XmlSerializer(typeof(Config));
                using (var fStream = new FileStream(Config.FileName, FileMode.Create))
                    serializer.Serialize(fStream, Config.Instance);

                return false;              
            }
        }

        // Saves the configuration to file.
        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Config));

            using (var fStream = new FileStream(Config.FileName, FileMode.Create))
                serializer.Serialize(fStream, this);
        }
    }
}