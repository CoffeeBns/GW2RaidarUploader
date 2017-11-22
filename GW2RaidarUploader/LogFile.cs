using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Will be used in the future to further granulate log uploads.
/// </summary>
namespace GW2RaidarUploader
{
    public class LogFile
    {
        public string filePath;
        public DateTime creationDate;
        public MainWindow.LogUploadStatus uploadStatus;

        public LogFile(string filePath, DateTime creationDate, MainWindow.LogUploadStatus uploadStatus)
        {
            this.filePath = filePath;
            this.creationDate = creationDate;
            this.uploadStatus = uploadStatus;
        }
    }
}
