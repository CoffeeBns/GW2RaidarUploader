using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
/// <summary>
/// Will be used in the future to further granulate log uploads.
/// </summary>
namespace GW2RaidarUploader
{
    [ProtoContract]
    public class LogFile
    {
        
        [ProtoMember(1)]
        public string filePath { get; set; }
        [ProtoMember(2)]
        public string encounter { get; set; }
        [ProtoMember(3)]
        public DateTime creationDate { get; set; }
        [ProtoMember(4)]
        public MainWindow.LogUploadStatus uploadStatus { get; set; }
        [ProtoMember(5)]
        public string uploadResponse { get; set; }
        [ProtoMember(6)]
        public string tag { get; set; }
        [ProtoMember(7)]
        public string dpsReportURL { get; set; }

        int _fontSize = 11;

        private ImageBrush _cachedIcon;

        public LogFile(string filePath, DateTime creationDate, string encounter, MainWindow.LogUploadStatus uploadStatus)
        {
            this.filePath = filePath;
            this.creationDate = creationDate;
            this.encounter = encounter;
            this.uploadStatus = uploadStatus;
            uploadResponse = "";
            tag = "";
            dpsReportURL = null;
        }


        public LogFile() {

            filePath = "";
            encounter = "";
            creationDate = DateTime.Now;
            uploadStatus = MainWindow.LogUploadStatus.NotUploaded;
            uploadResponse = "";
            tag = "";
            dpsReportURL = null;

            }

        public string DateStringShort {

            get
            {
                return creationDate.ToString("MM/dd/yy hh:mm");
            }
        }

        public string UploadStatusString
        {
            get
            {
                string status = "Unknown";

                if (uploadStatus == MainWindow.LogUploadStatus.FailedUpload)
                    status = "Upload Failed";
                else if (uploadStatus == MainWindow.LogUploadStatus.NotUploaded)
                    status = "Not Uploaded";
                else if (uploadStatus == MainWindow.LogUploadStatus.Uploaded)
                    status = "Uploaded";
                else if (uploadStatus == MainWindow.LogUploadStatus.UploadSkipped)
                    status = "Upload Skipped";

                return status;

            }
        }

    
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
        }

        public string FileName
        {
            get
            {
                if (File.Exists(filePath))
                {
                    return Path.GetFileName(filePath);
                }
                else
                    return filePath;
            }
        }

        public Visibility DPSReportVisibility
        {
            get
            {
                if (dpsReportURL != null)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;

            }

        }


        public ImageBrush Icon
        {
            get
            {
                if (_cachedIcon != null)
                    return _cachedIcon;

                try
                {
           
                    var iconName = encounter.Replace(" ", "_") + ".png";

                    if (iconName.Contains("Kitty_Golem"))
                        iconName = "Kitty_Golem.png";


                    var drawingGroup = new DrawingGroup();

                    if (File.Exists(@"Images/Icons/" + iconName))
                    {
                       

                        drawingGroup.Children.Add(new ImageDrawing(new BitmapImage(new Uri(@"Images/Icons/" + iconName, UriKind.Relative)),
                                                                   new Rect(0, 0, 25, 25)));


                    }
                    else
                    {
                       var newEncounter = ClientOperator.mainWindow.RaidEncountersDEtoEN[encounter];

                        iconName = newEncounter.Replace(" ", "_") + ".png";

                        if (iconName.Contains("Kitty_Golem"))
                            iconName = "Kitty_Golem.png";

                        if (File.Exists(@"Images/Icons/" + iconName))
                        {


                            drawingGroup.Children.Add(new ImageDrawing(new BitmapImage(new Uri(@"Images/Icons/" + iconName, UriKind.Relative)),
                                                                       new Rect(0, 0, 25, 25)));


                        }
                        else
                        {
                                iconName = "Choya_Pinata.png";
                                drawingGroup.Children.Add(new ImageDrawing(new BitmapImage(new Uri(@"Images/Icons/" + iconName, UriKind.Relative)),
                                    new Rect(0, 0, 25, 25)));

                        }

                    }
                    
                    
            
                    var brush = new ImageBrush { ImageSource = new DrawingImage(drawingGroup) };

                    _cachedIcon = brush;

                    return brush;
                }
                catch (Exception)
                {
                    return new ImageBrush();
                }
            }
        }
    }
}
