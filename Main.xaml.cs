/*
 *
 * author: jbuddha
 * timestamp: 4/23/2015 9:12 AM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;

using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;



namespace JarScanner
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : Window
    {
        Keeper keeper;
        private string currentScanPath = "";
        private readonly BackgroundWorker worker = new BackgroundWorker();

        string currentJar;
        int TotalJars, TotalFiles;
        public Main()
        {
            InitializeComponent();
            worker.DoWork += worker_Scan;
            worker.RunWorkerCompleted += worker_ScanCompleted;
            worker.ProgressChanged += worker_ScanProgressChanged;
            worker.WorkerReportsProgress = true;
        }
        
        void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            this.resultText.Text = keeper.ToString(textBox.Text);
        }

        void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.searchBox.GotFocus -= searchBox_GotFocus;
            this.searchBox.Text = "";
            
        }
        
        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            this.resultText.Text = "Search Results";
            this.searchBox.TextChanged += searchBox_TextChanged;
            this.searchBox.GotFocus += searchBox_GotFocus;
            var s = new StringBuilder();
            
            this.resultText.Text = s.ToString();
        }
        void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (dialog.SelectedPath.Length > 1) {
                pathLabel.Content = dialog.SelectedPath;
                currentScanPath = dialog.SelectedPath;
                scanProgress.Minimum = 0;
                scanProgress.Maximum = 100;
                scanProgress.Value = 0;
                worker.RunWorkerAsync();
            }
            
        }
        
       // To Refresh the UI immediately
        private delegate void RefreshDelegate();
        private static void Refresh(DependencyObject obj)
        {
            obj.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
                                  (RefreshDelegate)delegate { });
        }
        
        private void worker_Scan(object sender, DoWorkEventArgs e)
        {
            
            keeper = new Keeper();
            
            var Entries = new Dictionary<string, ArrayList>();
            string[] files = Directory.GetFiles(currentScanPath, "*.jar", SearchOption.AllDirectories);

            int count = files.Length;
            TotalJars = count;
            currentJar = "";
            worker.ReportProgress(0);
            int cValue = 0;
            foreach(string file in files){
                currentJar = file;
                worker.ReportProgress(100*cValue/count);
                Entries.Add(file,new ArrayList());
                Entries[file] = GetZipEntries(file);
                
                cValue++;
            }
            keeper.Entries = Entries;
            worker.ReportProgress(100);
            keeper.TotalJars = TotalJars;
            keeper.TotalFiles = TotalFiles;
            resultText.Text = keeper.ToString();
        }

        private void worker_ScanCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string s = "Total Jars: " + keeper.TotalJars + "             Total Files: " + keeper.TotalFiles;
            statusLabel.Content = s;
            resultText.Text = keeper.ToString();
        }
        
        private void worker_ScanProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            scanProgress.Value = e.ProgressPercentage;
            statusLabel.Content = currentJar;
        }
        
        private ArrayList GetZipEntries(String filePath)
        {
            var list = new ArrayList();
            try {
                using (var zipToOpen = new FileStream(filePath, FileMode.Open))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        foreach (var zipArchiveEntry in archive.Entries) {
                            if (zipArchiveEntry.CompressedLength > 0) {
                                list.Add(zipArchiveEntry.FullName);
                                TotalFiles++;
                            }
                        }
                    }
                }
            } catch (Exception) {
                
            }
            return list;
        }
    }
}