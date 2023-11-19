using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;

namespace PlanLekcji
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        private int x = 0;
        private readonly string pathToSave = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PlanLekcji", "path.txt");
        //IsolatedStorageFile storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        //private string sFileName;
        public MainWindow()
        {
            InitializeComponent();
            FileManagement();
        }

        private void OnMenuItemClicked(object sender, RoutedEventArgs e)
        {
            CreateFileDialog();
        }

        private void FileManagement()
        {
           // string pathToSave = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PlanLekcji", "path.txt");
            if (File.Exists(pathToSave) == false)
            {
                //File.CreateText("File.txt");
                AskFileOnFirstBoot();
            }
            else
            {
                try
                {
                    string s = File.ReadAllText(pathToSave);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(s, UriKind.Absolute);
                    bitmap.EndInit();

                    Plan.Source = bitmap;
                }
                catch
                {
                    MessageBox.Show("Plik nie istnieje! \nWybierz inny plik.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        private void AskFileOnFirstBoot()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PlanLekcji")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PlanLekcji"));
            }
            //string pathToSave = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PlanLekcji", "path.txt");
            StreamWriter s = File.CreateText(pathToSave);
            s.Close();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.png)|*.jpg;*.png|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                File.WriteAllText(pathToSave, path);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.EndInit();

                Plan.Source = bitmap;
            }

        }

        private void CreateFileDialog()
        {
           // string pathToSave = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PlanLekcji", "path.txt");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.png)|*.jpg;*.png|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string path = openFileDialog.FileName;

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.EndInit();

                    Plan.Source = bitmap;
                    File.WriteAllText(pathToSave, path);
                }
                catch
                {
                    MessageBox.Show("Zły format pliku, wybierz jpg albo png!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        private string SetImage(string p)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(p, UriKind.Absolute);
            bitmap.EndInit();

            Plan.Source = bitmap;
            return p; //todo zamienic to ze zwraca mi ladnie stringa i wykonuje kod od razu
        }

        private void OnScalingItemClicked(object sender, RoutedEventArgs e)
        {
            x = 1-x;
            switch (x)
            {
                case 0:
                    Plan.Stretch = Stretch.Uniform;
                    break;
                case 1:
                    Plan.Stretch = Stretch.Fill;
                    break;
            }
            //WebBrowser webView = new WebBrowser();
            //webView.Source = new Uri("https://google.com");
        }

        public void SetScalingOnBoot()
        {
           // MessageBox.Show("SetScalingOnBoot");
        }

        private void TrashcanClicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "shell:RecycleBinFolder");
        }

        private void SleepHandler(object sender, RoutedEventArgs e)
        {
            x = 1 - x;
            switch (x)
            {
                case 0:
                    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
                    SleepItem.Background = Brushes.Red;
                    SleepItem.Header = "Uśpienie? Nie";
                    break;
                case 1:
                    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
                    SleepItem.Background = Brushes.Green;
                    SleepItem.Header = "Uśpienie? Tak";
                    break;
            }
        }
    }
}
