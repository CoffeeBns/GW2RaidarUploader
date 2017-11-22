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
using System.Drawing;

namespace GW2RaidarUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
       
        string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GW2RaidarUploader";

        //Imported in from Config
        string raidarLogsPath = "";
        string username = "";
        string password = "";
        DateTime dateToUploadFrom = DateTime.Now;
        int syncRate = 5;
        Dictionary<string, int> logsDictionary = new Dictionary<string, int>();

        public enum LogUploadStatus
        {
            NotUploaded = 0,
            Uploaded = 1,
            FailedUpload = 2
        }

        bool autoSyncEnabled = false;
        bool doneUploading = true;
        int _completedUploads = 0;
        int totalFilesToUpload = 0;
        private readonly NotifyIcon systrayIcon;

        DateTime lastUpdateTick = DateTime.Now;

        DispatcherTimer syncTimer;
        

        public MainWindow()
        {
            InitializeComponent();
            ClientOperator.mainWindow = this;


            Config.Load();
            LoadConfig();
            LoadUI();

            if (Config.Instance.notifyOnSystray)
                SystrayCB.IsChecked = true;

            systrayIcon = new NotifyIcon
            {
                Icon = new Icon(@"Images/GW2RaidarUploaderIcon.ico"),
                Visible = true,
                ContextMenu = new ContextMenu(),
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

            if (autoSyncEnabled)
            {
                UploadButton_Click(this, null);
            }

        }

        private void syncTimer_Tick(object sender, EventArgs e)
        {
            if (autoSyncEnabled)
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
                autoSyncEnabled = c.autoSyncEnabled;
                logsDictionary = c.logsDictionary;
              
                if (dateToUploadFrom == DateTime.MinValue)
                    dateToUploadFrom = DateTime.Now;

                syncRate = c.syncRate;

                if (!Directory.Exists(raidarLogsPath))
                {
                    raidarLogsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Guild Wars 2\addons\arcdps\arcdps.cbtlogs");
                }

              

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

   

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
           
            _completedUploads = 0;
        
            doneUploading = false;
            autoSyncEnabled = true;
           
           
            ThreadStart threadStart = delegate
            {
                AttemptUpload();
            };

            new Thread(threadStart).Start();

        }

        bool UpdateAllValues()
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

        public void AttemptUpload()
        {


            if (!UpdateAllValues())
            {
                AddMessage("Missing values required to begin upload process. Please check them.");
                return;
            }

            LoadConfig();

            try
            {

                string[] allFiles = Directory.GetFiles(raidarLogsPath, "*.evtc*", SearchOption.AllDirectories);

                List<string> filesToUpload = new List<string>();
                AddMessage("Uploading all files from " + dateToUploadFrom.ToString() + " forward.");

                for (int i = 0; i < allFiles.Length; i++)
                {
                    if (File.GetCreationTime(allFiles[i]) >= dateToUploadFrom)
                    {
                        if (!logsDictionary.ContainsKey(allFiles[i]))
                        {
                            logsDictionary.Add(allFiles[i], (int)LogUploadStatus.NotUploaded);

                            filesToUpload.Add(allFiles[i]);
                        }
                        else if (logsDictionary[allFiles[i]] != (int)LogUploadStatus.Uploaded)
                        {
                            filesToUpload.Add(allFiles[i]);
                        }
                    }
                }

                DateTime syncStarted = DateTime.Now;

                this.Dispatcher.Invoke((Action)(() =>
                {
                    UploadButton.IsEnabled = false;
                    StopUploadButton.IsEnabled = true;
                    ProgressBarControl.Maximum = filesToUpload.Count;
                    ProgressBarControl.Value = 0;
                    Config.Instance.autoSyncEnabled = true;
                    Config.Save();
                
                }));

                totalFilesToUpload = filesToUpload.Count;

                for (int i = 0; i < totalFilesToUpload; i++)
                {
                    bool uploaded = UploadLogFile(filesToUpload[i]);
                    
                    if (uploaded)
                    {
                        logsDictionary[filesToUpload[i]] = (int)LogUploadStatus.Uploaded;
                    }
                    else
                    {
                        logsDictionary[filesToUpload[i]] = (int)LogUploadStatus.FailedUpload;
                        AddMessage("Syncing cancelled due to failed upload.");
                        doneUploading = true;
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

                Config.Instance.logsDictionary = logsDictionary;
                Config.Save();

                if (totalFilesToUpload > 0)
                {
                    AddMessage("All log files uploaded in " + (DateTime.Now - syncStarted).TotalSeconds + " seconds.");
                }

                CompletionMessage();
            }
            catch(Exception ex)
            {
                AddMessage("Failed to upload log files due to " + ex);
            }
        }

        private bool UploadLogFile(string file)
        {

            try
            {
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
                    }

                return true;
            }
            catch (Exception ex)
            {
                AddMessage("Failed to upload files due to " + ex);
                return false;
            }
        }

        private void CompletionMessage()
        {
            if (!doneUploading)
            {
                doneUploading = true;
     

                lastUpdateTick = DateTime.Now;
         
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (!autoSyncEnabled)
                    {
                        UploadButton.IsEnabled = true;
                        StopUploadButton.IsEnabled = false;
                    }
                    else
                    {
                        UploadButton.IsEnabled = false;
                        StopUploadButton.IsEnabled = true;
                    }

                    systrayIcon.Text = "GW2RaidarUploader - Last update " + lastUpdateTick.ToString();

                }));

                DateTime nextUpdateTime = lastUpdateTick;
                AddMessage("Next upload check will be at " + nextUpdateTime.AddMinutes(syncRate));

            }
        }

        #region StatusTextBox

        public void AddMessage(string msg)
        {
            string _now = DateTime.Now.ToString("hh:mm");
            Dispatcher.BeginInvoke(DispatcherPriority.Input, (ThreadStart)(
             () =>
             {
                 this.statusText.AppendText("[" + _now + "]" + msg + "\n");
                 statusText.ScrollToEnd();
             }));

        }

        #endregion

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

        private void StopUploadButton_Click(object sender, RoutedEventArgs e)
        {
            autoSyncEnabled = false;
            Config.Instance.autoSyncEnabled = false;
            Config.Save();

            this.Dispatcher.Invoke((Action)(() =>
            {
                if (!autoSyncEnabled)
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
            //logsDictionary = new Dictionary<string, int>();
            UploadButton_Click(this, null);
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

        private void SystrayCB_Checked(object sender, RoutedEventArgs e)
        {
            Config.Instance.notifyOnSystray = true;
            Config.Save();

        }

        private void SystrayCB_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.Instance.notifyOnSystray = false;
            Config.Save();
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

        public void ShowNotificationMessage(string _title, string _message)
        {

                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (Config.Instance.notifyOnSystray)
                    {
                        systrayIcon.BalloonTipTitle = _title;
                        systrayIcon.BalloonTipText = _message;
                        systrayIcon.ShowBalloonTip(10000);
                
                    }

                }));
            
        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Config.Save();

            systrayIcon.Visible = false;
            systrayIcon.BalloonTipClosed += (Tsender, Te) => { var thisIcon = (NotifyIcon)Tsender; thisIcon.Visible = false; thisIcon.Dispose(); };
            systrayIcon.Dispose();

        }
}
}
