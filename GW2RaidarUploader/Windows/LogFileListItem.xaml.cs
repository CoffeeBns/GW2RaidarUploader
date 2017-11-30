using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for LogFileListItem.xaml
    /// </summary>
    public partial class LogFileListItem
    {
        public LogFileListItem()
        {
            InitializeComponent();
        }

   

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (ClientOperator.logFileList != null)
            {
                ClientOperator.logFileList.UploadSelectedButton_Click(this, null);
            }
        }

        private void BtnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            if (ClientOperator.logFileList != null)
            {
                ClientOperator.logFileList.UnselectAllButton_Click(this, null);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientOperator.logFileList != null)
            {
                ClientOperator.logFileList.DeleteSelectedButton_Click();
            }
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (ClientOperator.logFileList != null)
            {
                ClientOperator.logFileList.SelectAllButton_Click(this, null);
            }
        }

        private void UserControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (ClientOperator.logFileList != null)
            {
                ClientOperator.logFileList.ListViewLogFiles.SelectedItems.Add(this);
            }

        }

         protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            return;
            //base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            return;
            //base.OnMouseRightButtonDown(e);
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ContextMenu.IsOpen)
                ContextMenu.IsOpen = true;
           
            e.Handled = true;
        }

        private void BtnDPSReport_Click(object sender, RoutedEventArgs e)
        {
            if (ClientOperator.logFileList != null)
            {
                ClientOperator.logFileList.UploadSelectedToDPSButton_Click(this, null);
            }
        }

        private void DpsReportLinkButton_Click(object sender, RoutedEventArgs e)
        {
            LogFile lf = DataContext as LogFile;

            Process.Start(lf.dpsReportURL);
        }
    }
}
