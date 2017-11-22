using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ProtoBuf;

namespace GW2RaidarUploader
{
    [ProtoContract]
    public class Config
    {

        private static Config _config;
        // private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 
        //New Business, Transfers, Telemarketers, Agreement Customers, Break Fix Customers, Other

        public readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                             + @"\GW2RaidarUploader";


        [ProtoMember(1)]
        [DefaultValue(true)]
        public bool? SaveConfigInAppData = true;

        [ProtoMember(2)]
        [DefaultValue(true)]
        public bool? SaveDataInAppData = true;

        [ProtoMember(3)]
        [DefaultValue("")]
        public string raidarLogsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Guild Wars 2\addons\arcdps\arcdps.cbtlogs");

        [ProtoMember(4)]
        [DefaultValue("")]
        public string username = "";

        [ProtoMember(5)]
        [DefaultValue("")]
        public string password = "";

        [ProtoMember(6)]
        [DefaultValue(null)]
        public Dictionary<string, int> logsDictionary = new Dictionary<string, int>();

        [ProtoMember(7)]
        [DefaultValue(".")]
        public string DataDirPath = ".";

        [ProtoMember(8)]
        [DefaultValue(null)]
        public DateTime dateToUploadFrom = DateTime.Now;

        [ProtoMember(9)]
        [DefaultValue(5)]
        public int syncRate = 5;

        [ProtoMember(10)]
        [DefaultValue(false)]
        public bool autoSyncEnabled = false;

        [ProtoMember(11)]
        [DefaultValue(false)]
        public bool runOnStartup = false;

        [ProtoMember(12)]
        [DefaultValue(false)]
        public bool notifyOnSystray = false;

        #region Config Controls


        public static void Load()
        {
            bool configExists = false;

            if (!File.Exists(Config.Instance.BackupDir))
            {
                Directory.CreateDirectory(Instance.BackupDir);
            }
            try
            {
              

                if (File.Exists(Instance.ConfigPath + "config.pbf"))
                {
                    using (var file = File.OpenRead(Instance.ConfigPath + "config.pbf"))
                        _config = Serializer.Deserialize<Config>(file);
                    configExists = true;
                }
                else if (File.Exists(Instance.AppDataPath + @"\config.pbf"))
                {
                    using (var file = File.OpenRead(Instance.AppDataPath + @"\config.pbf"))
                    {
                        _config = Serializer.Deserialize<Config>(file);
                        configExists = true;
                    }

                }
                else if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
                    Instance.SaveConfigInAppData = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n\n" + e.InnerException + "\n\n If you don't know how to fix this, please delete "
                                + Instance.ConfigPath, "Error loading config.pbf");
                Application.Current.Shutdown();
            }
            if (!configExists)
            {
                if (Instance.ConfigDir != string.Empty)
                {
                    Directory.CreateDirectory(Instance.ConfigPath);
                }

            }
            else if (File.Exists(Instance.AppDataPath + "config.pbf"))
            {
                SaveBackup(true);
                File.Move("config.pbf", Instance.ConfigPath);
            }
        }

        [AttributeUsage(AttributeTargets.All, Inherited = false)]
        private sealed class DefaultValueAttribute : Attribute
        {
            // This is a positional argument
            public DefaultValueAttribute(object value)
            {
                Value = value;
            }

            public object Value { get; private set; }
        }

        public void Reset(string name)
        {
            //TODO: Upgrade to use LINQ and not the property's name!!
            var original = GetType().GetFields().First(x => x.Name == name);
            var attr = (DefaultValueAttribute)original.GetCustomAttributes(typeof(DefaultValueAttribute), false).First();
            original.SetValue(this, attr.Value);
        }

        public void ResetAll()
        {
            foreach (var original in GetType().GetFields())
            {
                var attr = (DefaultValueAttribute)original.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault();
                if (attr != null)
                    original.SetValue(this, attr.Value);
            }
        }

        public string BackupDir
        {
            get { return Path.Combine(DataDir, @"Backups\"); }
        }

        public string ConfigPath
        {
            get { return Instance.ConfigDir; }
        }

        public string ConfigDir
        {
            get { return Instance.SaveConfigInAppData == false ? string.Empty : AppDataPath + @"\Config\"; }
        }

        public string DataDir
        {
            get { return Instance.SaveDataInAppData == false ? DataDirPath + "\\" : AppDataPath + "\\"; }
        }

   

        public static Config Instance
        {
            get
            {
                if (_config == null)
                {
                    _config = new Config();
                    _config.ResetAll();
                }

                return _config;
            }
        }

        public static void Save()
        {
            try
            {
                using (var file = File.Create(Instance.ConfigPath + "config.pbf"))
                {
                    Serializer.Serialize<Config>(file, Instance);
                }
            }
            catch (Exception e)
            {
            }

        }

        public static void SaveBackup(bool deleteOriginal = false)
        {
            var configPath = Instance.ConfigPath;

            if (File.Exists(configPath))
            {
                File.Copy(configPath, configPath + DateTime.Now.ToFileTime());
                if (deleteOriginal)
                    File.Delete(configPath);
            }
        }



        #endregion

    }
}
