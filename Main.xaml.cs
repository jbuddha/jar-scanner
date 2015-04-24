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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
        private readonly BackgroundWorker scanner = new BackgroundWorker();

        string currentJar;
        int TotalJars, TotalFiles;

        string appStoreFolder;

        public Main()
        {
            InitializeComponent();
            scanner.DoWork += worker_Scan;
            scanner.RunWorkerCompleted += worker_ScanCompleted;
            scanner.ProgressChanged += worker_ScanProgressChanged;
            scanner.WorkerReportsProgress = true;
            
            var userAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appStoreFolder = Path.Combine(userAppDataFolder, System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            
            if(!Directory.Exists(appStoreFolder))
                Directory.CreateDirectory(appStoreFolder);
            
            IEnumerable<string> files = Directory.EnumerateFiles(appStoreFolder, "*.bin", SearchOption.AllDirectories);
            foreach (string file in files) {
                savedScans.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }
        
        void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if(textBox.Text.Length > 2)
                this.resultText.Text = keeper.ToString(textBox.Text);
        }

        void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.searchBox.GotFocus -= searchBox_GotFocus;
            this.searchBox.Text = "";
                        this.searchBox.TextChanged += searchBox_TextChanged;
        }
        
        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            this.resultText.Text = "Search Results";

            this.searchBox.GotFocus += searchBox_GotFocus;
            this.scanNameTextbox.GotFocus += scanNameTextbox_GotFocus;
            this.scanNameTextbox.TextChanged += scanNameTextbox_TextChanged;
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
                scanner.RunWorkerAsync();
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
            currentJar = "Listing all jars in the given Path";
            string[] files = Directory.GetFiles(currentScanPath, "*.jar", SearchOption.AllDirectories);

            int count = files.Length;
            TotalJars = count;
            currentJar = "";
            scanner.ReportProgress(0);
            int cValue = 0;
            foreach(string file in files){
                currentJar = file;
                scanner.ReportProgress(100*cValue/count);
                Entries.Add(file,new ArrayList());
                Entries[file] = GetZipEntries(file);
                
                cValue++;
            }
            keeper.Entries = Entries;
            scanner.ReportProgress(100);
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
        void scanSaveButton_Click(object sender, RoutedEventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(appStoreFolder+"/"+scanNameTextbox.Text+".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, keeper);
            stream.Close();
        }
        void scanNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (scanNameTextbox.Text.Length > 0)
                scanSaveButton.IsEnabled = true;
            else
                scanSaveButton.IsEnabled = false;
        }
        void scanNameTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            scanNameTextbox.GotFocus -= scanNameTextbox_GotFocus;
            scanNameTextbox.Text = "";
        }
        void savedScans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var name = appStoreFolder + "/"+savedScans.SelectedValue + ".bin";
            statusLabel.Content = "Loading Saved Scan from " + name;
            Refresh(statusLabel);
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
            keeper = (Keeper) formatter.Deserialize(stream);
            statusLabel.Content = "Loading Completed displaying results";
            Refresh(statusLabel);
            stream.Close();
            statusLabel.Content = "Total Jars: " + keeper.TotalJars + "             Total Files: " + keeper.TotalFiles;
            resultText.Text = keeper.ToString();
        }
        
     }
}