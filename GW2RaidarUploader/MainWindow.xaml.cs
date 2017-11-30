using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Net;
using System.Windows.Threading;
using System.Threading;
using MahApps.Metro.Controls.Dialogs;
using GW2RaidarUploader;
using System.Net.Http;
using System.Collections.Specialized;
using System.Text;
using System.Net.Http.Headers;
using RestSharp;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Media;
using System.Windows.Controls.Primitives;

namespace GW2RaidarUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        //Enabled Commands and Buttons for Testing.
        bool devModeEnabled = false;

        string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GW2RaidarUploader";

        //Imported in from Config
        string raidarLogsPath = "";
        string username = "";
        string password = "";
        DateTime dateToUploadFrom = DateTime.Now;
        int syncRate = 5;
        Dictionary<string, int> logsDictionary = new Dictionary<string, int>();
        Dictionary<string, LogFile> logFilesDictionary = new Dictionary<string, LogFile>();

        ObservableCollection<LogFile> logFilesCollection = new ObservableCollection<LogFile>();

        bool loaded = false;

        public enum LogUploadStatus
        {
            NotUploaded = 0,
            Uploaded = 1,
            FailedUpload = 2,
            UploadSkipped = 3
        }

        public List<string> RaidEncounters = new List<string>()
        {
            "UNKNOWN",
            "Arkk",
            "Artsariiv",
            "Average Kitty Golem",
            "Berg",
            "Cairn the Indomitable",
            "Deimos",
            "Ensolyss",
            "Gorseval",
            "Keep Construct",
            "MAMA",
            "Massive Average Kitty Golem",
            "Massive Kitty Golem",
            "Massive Standard Kitty Golem",
            "Massive Vital Kitty Golem",
            "Matthias Gabrel",
            "McLeod the Silent",
            "Mursaat Overseer",
            "Narella",
            "Sabetha the Saboteur",
            "Samarog",
            "Siax",
            "Skorvald the Shattered",
            "Slothasor",
            "Standard Kitty Golem",
            "Tough Kitty Golem",
            "Vale Guardian",
            "Vital Kitty Golem",
            "Xera",
            "Xera (Post-Half)",
            "Zane"
        };

        public List<string> RaidEncountersDE = new List<string>()
        {
            "Samarog",
            "Mursaat-Aufseher",
            "Gorseval der Facettenreiche",
            "Cairn der Unbeugsame",
            "Tal-W�chter",
            "Deimos",
            "Gewaltiger K�tzchen-Golem",
            "Durchschnittlicher K�tzchen-Golem",
            "Sabetha die Saboteurin",
            "Festenkonstrukt",
            "Faultierion",
            "Matthias Gabrel",
            "Xera",
            "Standard-K�tzchen-Golem",
            "MAMA",
            "Artsariiv",
            "Skorvold der Zerschmetterte",
            "Ensolyss der endlosen Pein",
            "Albtraum-Oratuss"
        };

        public Dictionary<string, string> RaidEncountersDEtoEN = new Dictionary<string, string>()
        {
            {"Samarog", "Samarog" },
            {"Mursaat-Aufseher", "Mursaat Overseer" },
             {"Gorseval der Facettenreiche", "Gorseval"},
             {"Cairn der Unbeugsame", "Cairn the Indomitable"},
             {"Tal-W�chter", "Vale Guardian"},
             {"Deimos", "Deimos"},
             {"Gewaltiger K�tzchen-Golem", "Massive Kitty Golem"},
             {"Durchschnittlicher K�tzchen-Golem", "Average Kitty Golem"},
             {"Sabetha die Saboteurin", "Sabetha the Saboteur"},
             {"Festenkonstrukt", "Keep Construct"},
             {"Faultierion", "Slothasor"},
             {"Matthias Gabrel", "Matthias Gabrel"},
             {"Xera", "Xera"},
             {"Standard-K�tzchen-Golem", "Standard Kitty Golem"},
             {"MAMA", "MAMA"},
             {"Artsariiv", "Artsariiv"},
             {"Skorvold der Zerschmetterte", "Skorvold the Shattered"},
             {"Ensolyss der endlosen Pein", "Ensolyss"},
             {"Albtraum-Oratuss", "Siax"}
        };


        bool doneUploading = true;
        int _completedUploads = 0;
        int totalFilesToUpload = 0;
        private readonly NotifyIcon systrayIcon;

        DateTime lastMouseUpEvent = DateTime.MinValue;
        DateTime lastNotificationSound = DateTime.MinValue;

        DateTime lastUpdateTick = DateTime.Now;

        DispatcherTimer syncTimer;

        public ImageSource _titleWallpaper = new ImageSourceConverter().ConvertFromString("pack://application:,,,/GW2RaidarUploader;component/Images/UI/InnWP1.png") as ImageSource;
        public ImageSource _tabsWallpaper = new ImageSourceConverter().ConvertFromString("pack://application:,,,/GW2RaidarUploader;component/Images/UI/InnWP2.png") as ImageSource;
        public ImageSource _mainWallpaper;
        public ImageSource _gridWallpaper = new ImageSourceConverter().ConvertFromString("pack://application:,,,/GW2RaidarUploader;component/Images/UI/InnWP3.png") as ImageSource;
        private int _lastWallpaper = 4;

        private System.Windows.Media.Brush _mainWindowBGColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03161A"));

        private string OakWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/OakWP1.png";
        private string OakTabsWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/OakWP2.png";
        private string OakGridWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/OakWP3.png";

        private string InnWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/InnWP1.png";
        private string InnTabsWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/InnWP2.png";
        private string InnGridWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/InnWP3.png";

        private string SpoutWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/spoutWP1.png";
        private string SpoutTabsWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/spoutWP2.png";
        private string SpoutGridWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/spoutWP3.png";

        private string ShipWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/shipWP1.png";
        private string ShipTabsWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/shipWP2.png";
        private string ShipGridWPUri = "pack://application:,,,/GW2RaidarUploader;component/Images/UI/shipWP3.png";

        TabItem selectedTabItem;

        public ImageSource TitleWallpaper
        {
            get
            {
                return _titleWallpaper;
            }
            set
            {
                if (_titleWallpaper != value)
                {
                    _titleWallpaper = value;
                    OnPropertyChanged("TitleWallpaper");
                }
            }
        }

        public ImageSource TabsWallpaper
        {
            get
            {
                return _tabsWallpaper;
            }
            set
            {
                if (_tabsWallpaper != value)
                {
                    _tabsWallpaper = value;
                    OnPropertyChanged("TabsWallpaper");
                }
            }
        }

        public ImageSource MainWallpaper
        {
            get
            {
                return _mainWallpaper;
            }
            set
            {
                if (_mainWallpaper != value)
                {
                    _mainWallpaper = value;
                    OnPropertyChanged("MainWallpaper");
                }
            }
        }

        public ImageSource GridWallpaper
        {
            get
            {
                return _gridWallpaper;
            }
            set
            {
                if (_gridWallpaper != value)
                {
                    _gridWallpaper = value;
                    OnPropertyChanged("GridWallpaper");
                }
            }
        }

        public string GetMainWindowBGColor
        {
            get
            {       
                return "#040813";
            }
        }


        public Brush MainWindowBGColor
        {
            get
            {
                return _mainWindowBGColor;
            }
            set
            {
                if (_mainWindowBGColor != value)
                {
                    _mainWindowBGColor = value;
                    OnPropertyChanged("MainWindowBGColor");
                }
            }
        }


        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            

            MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GetMainWindowBGColor));

            Randomize_Wallpaper();

        }

        public MainWindow()
        {
            InitializeComponent();
            ClientOperator.mainWindow = this;

            ClientOperator.mainWindow.DataContext = this;

            Config.Load();
            LoadConfig();
            LoadUI();
 

     

            systrayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(@"Images/GW2RaidarUploaderIcon.ico"),
                Visible = true,
                ContextMenu = new System.Windows.Forms.ContextMenu(),
                Text = "GW2 Raidar Uploader"
            };

            systrayIcon.ContextMenu.MenuItems.Add("Show", (sender, args) => ActivateWindow());
            systrayIcon.ContextMenu.MenuItems.Add("Exit", (sender, args) => Close());

            systrayIcon.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                    ActivateWindow();
            };


            syncTimer = new System.Windows.Threading.DispatcherTimer();
            syncTimer.Tick += new EventHandler(syncTimer_Tick);
            syncTimer.Interval = new TimeSpan(0, 0, 10);
            syncTimer.Start();

            if (Config.Instance.autoSyncEnabled)
            {
                UploadButton_Click(this, null);
            }


            EnableDevFunctions();

            LocalSyncButton_Click(this, null);

            LoadLogsList();
            loaded = true;

            //ListAllLogFilePaths();
        }

        private void EnableDevFunctions()
        {
            if (devModeEnabled)
            {
                WipeDictionariesButton.Visibility = Visibility.Visible;
                LocalSyncButton.Visibility = Visibility.Visible;
                NotificationTestButton.Visibility = Visibility.Visible;
            }
            else
            {
                WipeDictionariesButton.Visibility = Visibility.Collapsed;
                LocalSyncButton.Visibility = Visibility.Collapsed;
                NotificationTestButton.Visibility = Visibility.Collapsed;
            }
        }

        private void syncTimer_Tick(object sender, EventArgs e)
        {
            if (Config.Instance.autoSyncEnabled)
            {
                DateTime nextUpdateTick = lastUpdateTick.AddMinutes(syncRate);
                if(DateTime.Now > nextUpdateTick)
                {
                    if(doneUploading)
                    UploadButton_Click(this, null);
                }

                //AddMessage("Next update is in " + (nextUpdateTick - DateTime.Now).TotalSeconds + " seconds.");
            }
        }

        void LoadConfig()
        {
            try
            {
                Config c = Config.Instance;
                username = c.username;
                password = c.password;
                raidarLogsPath = c.raidarLogsPath;
                dateToUploadFrom = c.dateToUploadFrom;
                logFilesDictionary = c.logFilesDictionary;
                logsDictionary = c.logsDictionary;

                if (logFilesDictionary == null)
                    logFilesDictionary = new Dictionary<string, LogFile>();

                if (logsDictionary == null)
                    logsDictionary = new Dictionary<string, int>();

                if (dateToUploadFrom == DateTime.MinValue)
                    dateToUploadFrom = DateTime.Now;

                

                syncRate = c.syncRate;

                if (!Directory.Exists(raidarLogsPath))
                {
                    raidarLogsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Guild Wars 2\addons\arcdps\arcdps.cbtlogs");
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    NotifyOnSystrayCB.IsChecked = (c.notifyOnSystray ? true : false);
                    OnlyLastEncounterCB.IsChecked = (c.onlyUploadFinalEncounters ? true : false);
                    NotificationSoundsCB.IsChecked = (c.notificationSounds ? true : false);
                    MinimizeToSystrayCB.IsChecked = (c.minimizeToSystray ? true : false);
                    RandomizeBackgroundsCB.IsChecked = (c.randomizedBackgrounds ? true : false);
                    UploadToDPSReportCB.IsChecked = (c.uploadToDPSReport ? true : false);
                }));

            }
            catch (Exception ex)
            {
                AddMessage("Failed to load config due to " + ex);
            }
        }

        void LoadUI()
        {
            try
            {

               
                RaidarLogsPathTB.Text = raidarLogsPath;
                raidarUsernameTB.Text = username;
                raidarPasswordTB.Password = password;
                DPSLogsUploadDateStartTB.Text = dateToUploadFrom.ToString("MM/dd/yy");
                raidarSyncRateTB.Text = syncRate + "";


            }
            catch (Exception e)
            {
                AddMessage("Failed to load UI due to " + e);
                
            }
        }
        
        public void LoadLogsList()
        {
            try
            {
                if (logFilesDictionary.Count > 0)
                {
                    logFilesCollection = new ObservableCollection<LogFile>(logFilesDictionary.Values);

                    LogFileListPanel.SetObservableCollection(logFilesCollection);

                }
            }
            catch { }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {

        
                _completedUploads = 0;

                doneUploading = false;
                Config.Instance.autoSyncEnabled = true;
                Config.Save();

                ClientOperator.mainWindow.AddMessage("Attempting Raidar Files Upload.");
                ThreadStart threadStart = delegate
                {
                    AttemptUpload();
                };

                new Thread(threadStart).Start();

        }

        public bool UpdateAllValues()
        {
            bool allValuesValid = true;
            this.Dispatcher.Invoke((Action)(() =>
            {

                Config c = Config.Instance;


                if (RaidarLogsPathTB.Text.Length > 0)
                {
                    if (Directory.Exists(RaidarLogsPathTB.Text))
                        c.raidarLogsPath = RaidarLogsPathTB.Text;
                    else
                    {
                        AddMessage("Invalid Raidar Logs Path.");
                        allValuesValid = false;
                    }
                }
                else
                {
                    AddMessage("Invalid Raidar Logs Path.");
                    allValuesValid = false;
                }

                if (raidarUsernameTB.Text.Length > 0)
                    c.username = raidarUsernameTB.Text;
                else
                {
                    AddMessage("Invalid GW2Raidar Username.");
                    allValuesValid = false;
                }

                if (raidarPasswordTB.Password.Length > 0)
                    c.password = raidarPasswordTB.Password;
                else
                {
                    AddMessage("Invalid GW2Raidar password.");
                    allValuesValid = false;
                }

                if (DPSLogsUploadDateStartTB.Text.Length > 0)
                {
                    DateTime startDate;

                    if (DateTime.TryParse(DPSLogsUploadDateStartTB.Text, out startDate))
                    {
                        dateToUploadFrom = startDate;
                        c.dateToUploadFrom = startDate;
                    }
                    else
                    {
                        AddMessage("Invalid Upload Start Date.");
                        allValuesValid = false;
                    }
                }
                else
                {
                    AddMessage("Invalid Upload Start Date.");
                    allValuesValid = false;
                }

                if (raidarSyncRateTB.Text.Length > 0)
                {
                    int syncRate;

                    if (int.TryParse(raidarSyncRateTB.Text, out syncRate))
                    {
                        c.syncRate = syncRate;
                    }
                    else
                    {
                        c.syncRate = 5;
                    }

                }
                else
                    c.syncRate = 5;

                if (allValuesValid)
                {
                    Config.Save();

                    LoadConfig();
      
                }

            }));


            return allValuesValid;
        }

        public void AttemptUpload(bool overrideLastEncounterSyncTime = false)
        {

            if (!UpdateAllValues())
            {
                AddMessage("Missing values required to begin upload process. Please check them.");
                return;
            }

            LoadConfig();

            try
            {

                List<string> filesToUpload = GetFilesToUpload(overrideLastEncounterSyncTime);

                if (filesToUpload == null)
                {
                    AddMessage("Failed to find any files to upload at this time.");

                    if (Config.Instance.autoSyncEnabled)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            UploadButton.IsEnabled = false;
                            StopUploadButton.IsEnabled = true;
                            ProgressBarControl.Maximum = 1;
                            ProgressBarControl.Value = 0;
                            Config.Save();

                        }));
                    }

                    return;
                }

                UploadAllFiles(filesToUpload);
              
            }
            catch(Exception ex)
            {
                AddMessage("Failed to upload log files due to " + ex);
            }
        }

        private List<string> GetFilesToUpload(bool overrideLastEncounterSyncTime = false)
        {
            try
            {

                string[] allFiles = Directory.GetFiles(raidarLogsPath, "*.*", SearchOption.AllDirectories);

                List<string> filesToUpload = new List<string>();
               
                List<LogFile> potentialLogFiles = new List<LogFile>();

                DateTime syncStarted = DateTime.Now;

                for (int i = 0; i < allFiles.Length; i++)
                {
                    if (devModeEnabled)
                        AddMessage("Parsing " + allFiles[i]);

                    if (!allFiles[i].Contains(".evtc"))
                        continue;

                    DateTime creationDate = File.GetLastWriteTime(allFiles[i]);

                    string encounter = "";

                    encounter += RaidEncounters.FirstOrDefault(w => allFiles[i].Contains(w));

                    LogUploadStatus logUploadStatus = LogUploadStatus.NotUploaded;

                    if (encounter == "")
                    {
                       encounter += RaidEncountersDE.FirstOrDefault(w => allFiles[i].Contains(w));
                    }

                    if(encounter == "")
                        encounter = new DirectoryInfo(allFiles[i]).Name;

                    if (logsDictionary.ContainsKey(allFiles[i]))
                    {
                        logUploadStatus = (LogUploadStatus)logsDictionary[allFiles[i]];
                    }

                    if (!logFilesDictionary.ContainsKey(allFiles[i]))
                    {
                        LogFile lf = new LogFile(allFiles[i], creationDate, encounter, logUploadStatus);
                        logFilesDictionary.Add(allFiles[i], lf);
                    }


                    LogFile logFile = logFilesDictionary[allFiles[i]];

                    if (logFile.encounter.Length < 3 || logFile.encounter.Contains(".evtc"))
                        logFile.encounter = encounter;

                   

                    //If uploading all encounters within date range, regardless of if it was the kill and/or last attempt on the boss.
                    if (!Config.Instance.onlyUploadFinalEncounters)
                    {
                        if (creationDate >= dateToUploadFrom)
                        {

                            if (logFilesDictionary[allFiles[i]].uploadStatus != LogUploadStatus.Uploaded || (logFilesDictionary[allFiles[i]].dpsReportURL == null && Config.Instance.uploadToDPSReport))
                            {
                                filesToUpload.Add(allFiles[i]);
                            }
                        }
                    }
                    else
                    {
                        potentialLogFiles.Add(logFilesDictionary[allFiles[i]]);
                    }

                }

                if (Config.Instance.onlyUploadFinalEncounters)
                {
                    potentialLogFiles.Sort((a, b) => (a.creationDate.CompareTo(b.creationDate)));

                    for (int i = 0; i < potentialLogFiles.Count; i++)
                    {
                        LogFile lf = potentialLogFiles[i];

                        if(devModeEnabled)
                        AddMessage("e: " + lf.encounter + " d: " + lf.creationDate);
                    }

                    if(devModeEnabled)
                    AddMessage("-----------");

                    for (int i = 0; i < potentialLogFiles.Count; i++)
                    {
                        LogFile lf = potentialLogFiles[i];

                        bool eligibleForUpload = false;

                        if (DateTime.Now > lf.creationDate.AddMinutes(20) || overrideLastEncounterSyncTime)
                        {
                            eligibleForUpload = true;
                 
                            var futureEncounters = potentialLogFiles.Where(d => d.encounter == lf.encounter && Math.Abs(ClientOperator.FastFloor((float)(d.creationDate - lf.creationDate).TotalHours)) < 6);

                            if(devModeEnabled)
                            AddMessage(futureEncounters.Count<LogFile>() + " potential encounters in range of this one.");

                            foreach (LogFile otherLF in futureEncounters)
                            {
                                //More encounters on this boss today, move to the next potential log file.
                                if (otherLF.encounter == lf.encounter && otherLF.creationDate > lf.creationDate)
                                {
                                    if(devModeEnabled)
                                    AddMessage(lf.encounter + " at " + lf.creationDate + " skipped due to " + otherLF.encounter + " at " + otherLF.creationDate);

                                    eligibleForUpload = false;
                                    break;
                                }
                            }
                        }

                        if (eligibleForUpload && lf.creationDate > dateToUploadFrom)                      
                        {
                            filesToUpload.Add(lf.filePath);

                            if(devModeEnabled)
                            AddMessage(lf.encounter + " at " + lf.creationDate + " will be uploaded from " + lf.filePath);
                        }
                    }

                    AddMessage("Out of " + potentialLogFiles.Count + " logs, " + filesToUpload.Count + " will be uploaded.");

                    Config.Instance.logsDictionary = logsDictionary;
                    Config.Instance.logFilesDictionary = logFilesDictionary;
                    Config.Save();

                    return (filesToUpload.Count > 0 ? filesToUpload : null);
                }

             
                //AddMessage("All log files synced locally in " + (DateTime.Now - syncStarted).TotalSeconds + " seconds.");

            }
            catch (Exception ex)
            {
                AddMessage("Failed to upload log files due to " + ex);
                return null;
            }

            return null;
        }

        public void UploadAllFiles(List<string> filesToUpload)
        {
            AddMessage("Uploading all files from " + dateToUploadFrom.ToString() + " forward.");

            DateTime syncStarted = DateTime.Now;

            if (Config.Instance.autoSyncEnabled)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    UploadButton.IsEnabled = false;
                    StopUploadButton.IsEnabled = true;
                    ProgressBarControl.Maximum = filesToUpload.Count;
                    ProgressBarControl.Value = 0;
                    Config.Save();

                }));
            }

            totalFilesToUpload = filesToUpload.Count;

            for (int i = 0; i < totalFilesToUpload; i++)
            {
                bool uploaded = UploadLogFile(filesToUpload[i]);

                if (uploaded)
                {
                    //logsDictionary[filesToUpload[i]] = (int)LogUploadStatus.Uploaded;
                    logFilesDictionary[filesToUpload[i]].uploadStatus = LogUploadStatus.Uploaded;
                }
                else
                {
                    //logsDictionary[filesToUpload[i]] = (int)LogUploadStatus.FailedUpload;
                    logFilesDictionary[filesToUpload[i]].uploadStatus = LogUploadStatus.FailedUpload;
                    AddMessage("Syncing cancelled due to failed upload.");
                    doneUploading = true;

                    Config.Instance.logFilesDictionary = logFilesDictionary;
                    Config.Save();

                    return;
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    _completedUploads += 1;
                    ProgressBarControl.Value = _completedUploads;
                    if (totalFilesToUpload > 0)
                    {
                        ClientOperator.mainWindow.AddMessage("Completed " + _completedUploads + "/" + totalFilesToUpload + " uploads.");

                        if (_completedUploads == totalFilesToUpload)
                            CompletionMessage();
                    }
                }));
            }

            if (Config.Instance.uploadToDPSReport)
            {
                BeginUploadAllFilesToDPS(filesToUpload);
            }

            Config.Instance.logFilesDictionary = logFilesDictionary;
            Config.Instance.logsDictionary = logsDictionary;
            Config.Save();

            AddMessage("All log files uploaded in " + (DateTime.Now - syncStarted).TotalSeconds + " seconds.");
            CompletionMessage();
        }

        public void BeginUploadAllFilesToDPS(List<string> filesToUpload)
        {
            ThreadStart threadStart = delegate
            {
                UploadAllFilesToDPS(filesToUpload);
            };

            new Thread(threadStart).Start();
        }

        public void UploadAllFilesToDPS(List<string> filesToUpload)
        {
            AddMessage("Uploading to dps.report.");

            DateTime syncStarted = DateTime.Now;

            if (Config.Instance.autoSyncEnabled)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    UploadButton.IsEnabled = false;
                    StopUploadButton.IsEnabled = true;
                    ProgressBarControl.Maximum = filesToUpload.Count;
                    ProgressBarControl.Value = 0;
                    Config.Save();

                }));
            }

            totalFilesToUpload = filesToUpload.Count;

            for (int i = 0; i < totalFilesToUpload; i++)
            {
                string uploaded = UploadLogFileToDPS(filesToUpload[i]);

                if (uploaded != null)
                {

                    logFilesDictionary[filesToUpload[i]].dpsReportURL = uploaded;
                }
                else
                {

                    AddMessage("Syncing cancelled due to failed upload to dps.report.");
                    doneUploading = true;

                    Config.Instance.logFilesDictionary = logFilesDictionary;
                    Config.Save();

                    return;
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    _completedUploads += 1;
                    ProgressBarControl.Value = _completedUploads;
                    if (totalFilesToUpload > 0)
                    {
                        ClientOperator.mainWindow.AddMessage("Completed " + _completedUploads + "/" + totalFilesToUpload + " uploads.");

                        if (_completedUploads == totalFilesToUpload)
                            CompletionMessage();
                    }
                }));
            }

            Config.Instance.logFilesDictionary = logFilesDictionary;
            Config.Instance.logsDictionary = logsDictionary;
            Config.Save();

            LoadLogsList();
        }

        private bool UploadLogFile(string file)
        {

            try
            {

                LogFile lf = logFilesDictionary[file];

                if (lf.uploadStatus == LogUploadStatus.Uploaded)
                    return true;
            
                var client = new RestClient(@"https://www.gw2raidar.com");


                RestRequest req = new RestRequest(@"/api/upload.json");
                req.AddFile("file", file);
                req.AddParameter("username", username);
                req.AddParameter("password", password);
                
                //Comment this out to test in a way that gives false positive uploads.
                req.Method = Method.POST;

                IRestResponse response = client.Execute(req);

                    if (response.ResponseStatus == ResponseStatus.Completed)
                    {
                        AddMessage("Successfully uploaded " + file);
                        ShowNotificationMessage("Log Uploaded Successfully.", file + " has been uploaded.");
                        AddMessage(response.Content);
                    }
                    else
                    {
                        AddMessage("Failed to upload " + file);
                        return false;
                    }

                return true;
            }
            catch (Exception ex)
            {
                AddMessage("Failed to upload files due to " + ex);
                return false;
            }
        }

        private string UploadLogFileToDPS(string file)
        {

            try
            {
                var client = new RestClient(@"https://dps.report");

                LogFile lf = logFilesDictionary[file];

                if (lf.dpsReportURL != null)
                    return lf.dpsReportURL;

                RestRequest req = new RestRequest(@"/uploadContent");
                req.AddFile("file", file);

                //Comment this out to test in a way that gives false positive uploads.
                req.Method = Method.POST;

                IRestResponse response = client.Execute(req);

                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    AddMessage("Successfully uploaded " + file);
                    ShowNotificationMessage("Log Uploaded Successfully.", file + " has been uploaded.");
                }
                else
                {
                    AddMessage("Failed to upload " + file);
                    return null;
                }

                string url = response.Content;

                var urlHttp = url.Substring(url.LastIndexOf("https:"), url.Length - url.LastIndexOf("https:"));
            
                var finalURL = urlHttp.Replace(@"</a>", "");

                return finalURL;
            }
            catch (Exception ex)
            {
                AddMessage("Failed to upload files due to " + ex);
                return null;
            }
        }
        //Called upon upload Completion
        private void CompletionMessage()
        {
            if (!doneUploading)
            {
                doneUploading = true;
     

                lastUpdateTick = DateTime.Now;
         
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (!Config.Instance.autoSyncEnabled)
                    {
                        UploadButton.IsEnabled = true;
                        StopUploadButton.IsEnabled = false;
                    }
                    else
                    {
                        UploadButton.IsEnabled = false;
                        StopUploadButton.IsEnabled = true;
                    }

                    systrayIcon.Text = "GW2RaidarUploader - Last upload sync " + lastUpdateTick.ToString();

                }));

                DateTime nextUpdateTime = lastUpdateTick;
                AddMessage("Next upload check will be at " + nextUpdateTime.AddMinutes(syncRate));

                LoadLogsList();
            }
        }

        public void AddMessage(string msg)
        {
            string _now = DateTime.Now.ToString("hh:mm");
            Dispatcher.BeginInvoke(DispatcherPriority.Input, (ThreadStart)(
             () =>
             {
                 this.statusText.AppendText("[" + _now + "] " + msg + "\n");
                 statusText.ScrollToEnd();
             }));

        }

        void OptionsUpdate()
        {
            if (loaded)
            {
                Config c = Config.Instance;

                c.notifyOnSystray = (NotifyOnSystrayCB.IsChecked == true ? true : false);
                c.onlyUploadFinalEncounters = (OnlyLastEncounterCB.IsChecked == true ? true : false);
                c.notificationSounds = (NotificationSoundsCB.IsChecked == true ? true : false);
                c.minimizeToSystray = (MinimizeToSystrayCB.IsChecked == true ? true : false);
                c.randomizedBackgrounds = (RandomizeBackgroundsCB.IsChecked == true ? true : false);
                c.uploadToDPSReport = (UploadToDPSReportCB.IsChecked == true ? true : false);

                Config.Save();
            }
        }


        void ListAllLogFilePaths()
        {
            if(logFilesDictionary != null)
            {
                foreach(LogFile lf in logFilesDictionary.Values)
                {
                    AddMessage("path: " + lf.filePath);
                }
            }
        }
        #region Events and Triggers

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
        {
            public NotifyPropertyChangedInvocatorAttribute()
            {
            }

            public NotifyPropertyChangedInvocatorAttribute(string parameterName)
            {
                ParameterName = parameterName;
            }

            public string ParameterName { get; private set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private void StopUploadButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.autoSyncEnabled = false;   
            Config.Save();

            this.Dispatcher.Invoke((Action)(() =>
            {
                if (!Config.Instance.autoSyncEnabled)
                {
                    UploadButton.IsEnabled = true;
                    StopUploadButton.IsEnabled = false;
                }
                else
                {
                    UploadButton.IsEnabled = false;
                    StopUploadButton.IsEnabled = true;
                }

            }));


        }
     
        private void SyncNowButton_Click(object sender, RoutedEventArgs e)
        {
           
                _completedUploads = 0;

                doneUploading = false;

                ClientOperator.mainWindow.AddMessage("Attempting Single Raidar Files Upload.");
                ThreadStart threadStart = delegate
                {
                    AttemptUpload(true);
                };

                new Thread(threadStart).Start();
          
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Config.Instance.notifyOnSystray)
                MinimizeToTray();
            else
                WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (Config.Instance.notifyOnSystray && WindowState == WindowState.Minimized)
                MinimizeToTray();
        }

        private void MinimizeToTray()
        {
            systrayIcon.Visible = true;
            Hide();
            Visibility = Visibility.Collapsed;
            ShowInTaskbar = false;
        }

        public void ActivateWindow()
        {
            try
            {
                Show();
                ShowInTaskbar = true;
                Activate();
                WindowState = WindowState.Normal;
            }
            catch (Exception ex)
            {
               AddMessage("[UI]Error activating window: " + ex.ToString());
            }
        }

        public void ShowNotificationMessage(string _title, string _message, int _sound = 3)
        {

                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (Config.Instance.notifyOnSystray)
                    {
                        systrayIcon.BalloonTipTitle = _title;
                        systrayIcon.BalloonTipText = _message;
                        systrayIcon.ShowBalloonTip(10000);
                
                    }

                    if (Config.Instance.notificationSounds && DateTime.Now > lastNotificationSound.AddSeconds(15))
                    {
                        if (_sound == 0)
                        {
                            var player = new SoundPlayer();
                            player.Stream = Properties.Resources.notification;
                            
                            player.Play();
                        }
                        if (_sound == 1)
                        {
                            var player = new SoundPlayer();
                            player.Stream = Properties.Resources.notification1;
                            player.Play();
                        }
                        else if (_sound == 2)
                        {
                            var player = new SoundPlayer();
                            player.Stream = Properties.Resources.notification2;
                            player.Play();
                        }
                        else if (_sound == 3)
                        {
                            var player = new SoundPlayer();
                            player.Stream = Properties.Resources.notification3;
                            player.Play();
                        }


                        lastNotificationSound = DateTime.Now;
                    }
                }));
            
        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Config c = Config.Instance;
            c.logFilesDictionary = logFilesDictionary;
            Config.Save();

            systrayIcon.Visible = false;
            systrayIcon.BalloonTipClosed += (Tsender, Te) => { var thisIcon = (NotifyIcon)Tsender; thisIcon.Visible = false; thisIcon.Dispose(); };
            systrayIcon.Dispose();

        }

        //For Testing and eventual for manually viewing all logs and selecting individual ones to upload.
        private void LocalSyncButton_Click(object sender, RoutedEventArgs e)
        {    
   
            GetFilesToUpload();

            LoadConfig();

           
        }

        private void WipeDictionariesButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.logFilesDictionary = new Dictionary<string, LogFile>();
            Config.Instance.logsDictionary = new Dictionary<string, int>();
            logsDictionary = new Dictionary<string, int>();
            logFilesDictionary = new Dictionary<string, LogFile>();
            Config.Save();

            AddMessage(logsDictionary.Count + " logs in dict. " + Config.Instance.logFilesDictionary.Count + " logFiles in config.");
        }


        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (MainTabControl.SelectedItem != selectedTabItem && Config.Instance.randomizedBackgrounds)
            {
                Randomize_Wallpaper();
                LoadLogsList();

            }
            else
            {
                if (MainTabControl.SelectedItem == AutoSyncTabItem)
                {
                    MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17161B"));
                    TitleWallpaper = new BitmapImage(new Uri(OakWPUri));
                    TabsWallpaper = new BitmapImage(new Uri(OakTabsWPUri));
                    GridWallpaper = new BitmapImage(new Uri(OakGridWPUri));
                }
                else if(MainTabControl.SelectedItem == ViewLogsTabItem)
                {
                    MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#040813"));
                    TitleWallpaper = new BitmapImage(new Uri(InnWPUri));
                    TabsWallpaper = new BitmapImage(new Uri(InnTabsWPUri));
                    GridWallpaper = new BitmapImage(new Uri(InnGridWPUri));
                }
                else if(MainTabControl.SelectedItem == OptionsTabItem)
                {
                    MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#070C1A"));
                    TitleWallpaper = new ImageSourceConverter().ConvertFromString(ShipWPUri) as ImageSource;
                    TabsWallpaper = new ImageSourceConverter().ConvertFromString(ShipTabsWPUri) as ImageSource;
                    GridWallpaper = new ImageSourceConverter().ConvertFromString(ShipGridWPUri) as ImageSource;
                }
                else
                {
                    MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#211020"));
                    TitleWallpaper = new ImageSourceConverter().ConvertFromString(SpoutWPUri) as ImageSource;
                    TabsWallpaper = new ImageSourceConverter().ConvertFromString(SpoutTabsWPUri) as ImageSource;
                    GridWallpaper = new ImageSourceConverter().ConvertFromString(SpoutGridWPUri) as ImageSource;
                }

            }

            selectedTabItem = (TabItem)MainTabControl.SelectedItem;
        }

        private void Randomize_Wallpaper()
        {
      
            int _wallpaper = ClientOperator.RandomRangeNotRepeating(1, 4, _lastWallpaper);

            _lastWallpaper = _wallpaper;

            if (_wallpaper == 1)
            {
                MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#040813"));
                TitleWallpaper = new BitmapImage(new Uri(InnWPUri));
                TabsWallpaper = new BitmapImage(new Uri(InnTabsWPUri));
                GridWallpaper = new BitmapImage(new Uri(InnGridWPUri));
            }
            else if (_wallpaper == 2)
            {
                MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17161B"));
                TitleWallpaper = new BitmapImage(new Uri(OakWPUri));
                TabsWallpaper = new BitmapImage(new Uri(OakTabsWPUri));
                GridWallpaper = new BitmapImage(new Uri(OakGridWPUri));
            }
            else if (_wallpaper == 3)
            {
                MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#070C1A"));
                TitleWallpaper = new ImageSourceConverter().ConvertFromString(ShipWPUri) as ImageSource;
                TabsWallpaper = new ImageSourceConverter().ConvertFromString(ShipTabsWPUri) as ImageSource;
                GridWallpaper = new ImageSourceConverter().ConvertFromString(ShipGridWPUri) as ImageSource;
            }
            else if (_wallpaper == 4)
            {
                MainWindowBGColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#211020"));
                TitleWallpaper = new ImageSourceConverter().ConvertFromString(SpoutWPUri) as ImageSource;
                TabsWallpaper = new ImageSourceConverter().ConvertFromString(SpoutTabsWPUri) as ImageSource;
                GridWallpaper = new ImageSourceConverter().ConvertFromString(SpoutGridWPUri) as ImageSource;
            }

           
    }

        private void PseudoTitlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
          
            if (e.ChangedButton == MouseButton.Left)
            {            
                this.DragMove();
            }
        }

        void ToggleWindowMaximize()
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void ProgressBarControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void PseudoTitlebar_MouseUp(object sender, MouseButtonEventArgs e)
        {
           
            if((DateTime.Now - lastMouseUpEvent).TotalMilliseconds < 250)
            {
                ToggleWindowMaximize();
            }

            lastMouseUpEvent = DateTime.Now;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleWindowMaximize();
        }

        private void NotificationTestButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNotificationMessage("Test", "testing sound.");
        }

      
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OptionsUpdate();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            OptionsUpdate();
        }

        public async void PromptFileDeletion(bool filesNotUploaded)
        {
            string message = "Are you sure you want to delete " + ClientOperator.logFileList.ListViewLogFiles.SelectedItems.Count + " " + (ClientOperator.logFileList.ListViewLogFiles.SelectedItems.Count > 1 ? "files" : "file") + "?";

            if (filesNotUploaded)
                message = "Are you sure you want to delete " + ClientOperator.logFileList.ListViewLogFiles.SelectedItems.Count + " " + (ClientOperator.logFileList.ListViewLogFiles.SelectedItems.Count > 1 ? "files" : "file") + ", some of them have not been uploaded?";

            var promptResult = await this.ShowMessageAsync("Delete File", message, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No", ColorScheme = MetroDialogColorScheme.Accented });

            if (promptResult == MessageDialogResult.Affirmative)
            {
                ClientOperator.logFileList.DeleteSelectedFiles();
            }
        }


        public void DeleteFile(LogFile lf)
        {
            if(logFilesDictionary.ContainsKey(lf.filePath))
            logFilesDictionary.Remove(lf.filePath);

            if(logsDictionary.ContainsKey(lf.filePath))
            logsDictionary.Remove(lf.filePath);

            try
            {
                if (File.Exists(lf.filePath))
                {
                    File.Delete(lf.filePath);
                }

                Config.Instance.logFilesDictionary = logFilesDictionary;
                Config.Instance.logsDictionary = logsDictionary;

                Config.Save();

            }
            catch(Exception ex)
            {
                AddMessage("Failed to delete file " + lf.filePath + " due to " + ex);
            }

        }


        #endregion


       
    }
}
