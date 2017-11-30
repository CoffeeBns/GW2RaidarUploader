using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GW2RaidarUploader.Windows
{
    /// <summary>
    /// Interaction logic for LogFileList.xaml
    /// </summary>
    public partial class LogFileList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        SortPriority currentSortFilter = SortPriority.Encounter;
        bool ascending = false;

        ObservableCollection<LogFile> collection = new ObservableCollection<LogFile>();

        enum SortPriority {
            Encounter,
            EncounterDate,
            UploadStatus,
            Tag,
            FileName
        }


        public LogFileList()
        {
            InitializeComponent();
            ClientOperator.logFileList = this;
        }

        public void SetObservableCollection(ObservableCollection<LogFile> newCollection)
        {
            collection.Clear();

            for(int i = 0; i < newCollection.Count; i++)
            {
                collection.Add(newCollection[i]);
            }

            ListViewLogFiles.ItemsSource = collection;

            RepeatSort(collection);
            
        }

        private void EncounterSortButton_Click(object sender, RoutedEventArgs e)
        {
            SortByEncounter(collection, ascending);
        }

        private void EncounterDateSortButton_Click(object sender, RoutedEventArgs e)
        {
            SortByEncounterDate(collection, ascending);
        }

        private void UploadStatusSortButton_Click(object sender, RoutedEventArgs e)
        {
            SortByUploadStatus(collection, ascending);
        }

        private void TagSortButton_Click(object sender, RoutedEventArgs e)
        {
            SortByTag(collection, ascending);
        }

        private void FileNameSortButton_Click(object sender, RoutedEventArgs e)
        {
            SortByFileName(collection, ascending);
        }

        public void RepeatSort(IEnumerable collection)
        {
            if (currentSortFilter == SortPriority.Encounter)
                SortByEncounter(collection, !ascending);
            else if (currentSortFilter == SortPriority.EncounterDate)
                SortByEncounterDate(collection, !ascending);
            else if (currentSortFilter == SortPriority.UploadStatus)
                SortByUploadStatus(collection, !ascending);
        }

        public void SortByEncounter(IEnumerable collection, bool isAscending)
        {
            if (collection == null)
                return;
            var view1 = (CollectionView)CollectionViewSource.GetDefaultView(collection);
            view1.SortDescriptions.Clear();

            if (currentSortFilter != SortPriority.Encounter)
                isAscending = false;

            ListSortDirection lsd = (isAscending ? ListSortDirection.Descending : ListSortDirection.Ascending);

            view1.SortDescriptions.Add(new SortDescription("encounter", lsd));
            view1.SortDescriptions.Add(new SortDescription("creationDate", ListSortDirection.Ascending));
            view1.SortDescriptions.Add(new SortDescription("uploadStatus", ListSortDirection.Ascending));

            if(currentSortFilter == SortPriority.Encounter)
                ascending = (isAscending ? false : true);

            currentSortFilter = SortPriority.Encounter;
            

        }

        public void SortByEncounterDate(IEnumerable collection, bool isAscending)
        {
            if (collection == null)
                return;
            var view1 = (CollectionView)CollectionViewSource.GetDefaultView(collection);
            view1.SortDescriptions.Clear();

            if (currentSortFilter != SortPriority.EncounterDate)
                isAscending = true;

            ListSortDirection lsd = (isAscending ? ListSortDirection.Descending : ListSortDirection.Ascending);

            view1.SortDescriptions.Add(new SortDescription("creationDate", lsd));
            view1.SortDescriptions.Add(new SortDescription("encounter", ListSortDirection.Ascending));
            view1.SortDescriptions.Add(new SortDescription("uploadStatus", ListSortDirection.Ascending));

            if (currentSortFilter == SortPriority.EncounterDate)
                ascending = (isAscending ? false : true);

            currentSortFilter = SortPriority.EncounterDate;
         

        }

        public void SortByUploadStatus(IEnumerable collection, bool isAscending)
        {
            if (collection == null)
                return;
            var view1 = (CollectionView)CollectionViewSource.GetDefaultView(collection);
            view1.SortDescriptions.Clear();

            if (currentSortFilter != SortPriority.UploadStatus)
                isAscending = true;

            ListSortDirection lsd = (isAscending ? ListSortDirection.Descending : ListSortDirection.Ascending);

            view1.SortDescriptions.Add(new SortDescription("uploadStatus", lsd));
            view1.SortDescriptions.Add(new SortDescription("encounter", ListSortDirection.Ascending));
            view1.SortDescriptions.Add(new SortDescription("creationDate", ListSortDirection.Ascending));

            if (currentSortFilter == SortPriority.UploadStatus)
                ascending = (isAscending ? false : true);

            currentSortFilter = SortPriority.UploadStatus;
           

        }

        public void SortByTag(IEnumerable collection, bool isAscending)
        {
            if (collection == null)
                return;
            var view1 = (CollectionView)CollectionViewSource.GetDefaultView(collection);
            view1.SortDescriptions.Clear();

            if (currentSortFilter != SortPriority.Tag)
                isAscending = true;

            ListSortDirection lsd = (isAscending ? ListSortDirection.Descending : ListSortDirection.Ascending);

            view1.SortDescriptions.Add(new SortDescription("tag", lsd));
            view1.SortDescriptions.Add(new SortDescription("encounter", ListSortDirection.Ascending));
            view1.SortDescriptions.Add(new SortDescription("creationDate", ListSortDirection.Ascending));

            if (currentSortFilter == SortPriority.Tag)
                ascending = (isAscending ? false : true);

            currentSortFilter = SortPriority.Tag;

        }

        public void SortByFileName(IEnumerable collection, bool isAscending)
        {
            if (collection == null)
                return;
            var view1 = (CollectionView)CollectionViewSource.GetDefaultView(collection);
            view1.SortDescriptions.Clear();

            if (currentSortFilter != SortPriority.FileName)
                isAscending = true;

            ListSortDirection lsd = (isAscending ? ListSortDirection.Descending : ListSortDirection.Ascending);

            view1.SortDescriptions.Add(new SortDescription("FileName", lsd));
            view1.SortDescriptions.Add(new SortDescription("encounter", ListSortDirection.Ascending));
            view1.SortDescriptions.Add(new SortDescription("creationDate", ListSortDirection.Ascending));

            if (currentSortFilter == SortPriority.FileName)
                ascending = (isAscending ? false : true);

            currentSortFilter = SortPriority.FileName;

        }

        public void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (collection == null)
                return;

            ListViewLogFiles.SelectAll();


        }


        public void UploadSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (collection == null)
                return;

            if (ListViewLogFiles.SelectedItems.Count > 0)
            {
                if (ClientOperator.mainWindow.UpdateAllValues())
                {
                    var selectedItems = ListViewLogFiles.SelectedItems;

                    List<string> itemsToUpload = new List<string>();

                    foreach(var item in selectedItems)
                    {
                        LogFile lf = (item as LogFile);
                        if (lf.uploadStatus != MainWindow.LogUploadStatus.Uploaded || (Config.Instance.uploadToDPSReport && lf.dpsReportURL == ""))
                            itemsToUpload.Add((item as LogFile).filePath);
                    }

                    ClientOperator.mainWindow.UploadAllFiles(itemsToUpload);
                }
            }

        }

        public void UploadSelectedToDPSButton_Click(object sender, RoutedEventArgs e)
        {
            if (collection == null)
                return;

            if (ListViewLogFiles.SelectedItems.Count > 0)
            {
                if (ClientOperator.mainWindow.UpdateAllValues())
                {
                    var selectedItems = ListViewLogFiles.SelectedItems;

                    List<string> itemsToUpload = new List<string>();

                    foreach (var item in selectedItems)
                    {
                        if ((item as LogFile).uploadStatus != MainWindow.LogUploadStatus.Uploaded)
                            itemsToUpload.Add((item as LogFile).filePath);
                    }

                    ClientOperator.mainWindow.BeginUploadAllFilesToDPS(itemsToUpload);
                }
            }

        }

        public void UnselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (collection == null)
                return;

            ListViewLogFiles.UnselectAll();
        }

        public void DeleteSelectedButton_Click()
        {
            if (collection == null)
                return;

            bool filesNotUploaded = false;

            var selectedItems = ListViewLogFiles.SelectedItems;

            foreach (var item in selectedItems)
            {
                if ((item as LogFile).uploadStatus != MainWindow.LogUploadStatus.Uploaded)
                {
                    filesNotUploaded = true;
                    break;
                }
            }

            ClientOperator.mainWindow.PromptFileDeletion(filesNotUploaded);


        }

        public void DeleteSelectedFiles()
        {
            var selectedItems = ListViewLogFiles.SelectedItems;

            foreach (var item in selectedItems)
            {
                LogFile lf = (item as LogFile);

                ClientOperator.mainWindow.DeleteFile(lf);
               
            }

            ClientOperator.mainWindow.LoadLogsList();
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            //base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            //base.OnMouseRightButtonDown(e);
        }


    }
}
