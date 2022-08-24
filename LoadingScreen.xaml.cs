
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace InDevLauncher
{

    /// <summary>
    /// Logique d'interaction pour LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : Window, INotifyPropertyChanged
    {

        DispatcherTimer dT = new DispatcherTimer();
        private BackgroundWorker _bgWorker = new BackgroundWorker();
        private int _workerState;


        public int workerState
        {
            get { return _workerState; }
            set 
            {
                _workerState = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("WorkerState"));
            }
        }

        public string pathInDev = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\AppData\Roaming\IndevLauncher\";

        public LoadingScreen()
        {
            InitializeComponent();

            dwnldActu();
            dwnldVersion();
            SetVersion();

            DataContext = this;

            _bgWorker.DoWork += (s, e) =>
            {
                for(int i= 0; i <= 100; i++)
                {
                    System.Threading.Thread.Sleep(100);
                    workerState= i;
                }
            };
                _bgWorker.RunWorkerAsync();

            Random rnd = new Random();
            int loadTime = 12;

#pragma warning disable CS8622
            dT.Tick += new EventHandler(dt_Tick);
#pragma warning restore CS8622
            dT.Interval = new TimeSpan(0, 0, loadTime);
            dT.Start();
        }

        #region INotifyProperetyChanged Member

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        private void dt_Tick(object sender, EventArgs e)
        {
            MainWindow db = new MainWindow();
            db.Show();

            dT.Stop();
            this.Close();
        }

        private void prgss_bar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        public void SetVersion()
        {
            try
            {
                string url = "https://indevs.000webhostapp.com/misc/version.txt";
                HttpClient client = new HttpClient();
                {
                    using (HttpResponseMessage reponse = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = reponse.Content)
                        {
                            var json = content.ReadAsStringAsync().Result;
                            lblVersion.Content = json;
                        }
                    }
                }
            }
            catch
            {
                File.ReadAllText($"{pathInDev}versionClient.txt");
                File.ReadAllText($"{pathInDev}versionClient.txt");
            }
        }

        public void dwnldActu()
        {
            try
            {
                string url = "https://indevs.000webhostapp.com/misc/actu.txt";
                HttpClient client = new HttpClient();
                {
                    using (HttpResponseMessage reponse = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = reponse.Content)
                        {
                            var json = content.ReadAsStringAsync().Result;
                            File.WriteAllText($"{pathInDev}actuClient.txt", json);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }


        public void dwnldVersion()
        {
            try
            {
                string url = "https://indevs.000webhostapp.com/misc/version.txt";
                HttpClient client = new HttpClient();
                {
                    using (HttpResponseMessage reponse = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = reponse.Content)
                        {
                            string json = content.ReadAsStringAsync().Result;
                            
                            File.WriteAllText($"{pathInDev}versionClient.txt", json);
                        }
                    }
                }
            }
            catch
            {
                File.AppendAllText($"{pathInDev}versionClient.txt", "");
                lblVersion.Content = File.ReadAllText($"{pathInDev}versionClient.txt");
            }
        }
    }
}