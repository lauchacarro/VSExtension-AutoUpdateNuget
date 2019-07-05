using System;
using System.IO;
using System.Windows;
using VSIXProject1.Settings.Nuget;


namespace VSIXProject1.Settings.Form
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string SETTINGFILE = "supernugetupdate.settings";
        string fullPath = Path.Combine(Path.GetTempPath(), SETTINGFILE);
        public NugetSetting Settings;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(string pathProject)
        {
            InitializeComponent();
            LoadSettings();
            Settings.Package = pathProject;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings.Server = txtNugetServer.Text;
            Settings.ApiKey = txtApiKey.Text;
            Settings.IncrementarVersion = chkAutoVersion.IsChecked.Value;

            NugetUpdate nugetUpdate = new NugetUpdate();
            try
            {
                nugetUpdate.Execute(Settings);
                SaveSettings();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hubo un error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }

            Close();
        }

        private void LoadSettings()
        {
            if (File.Exists(fullPath))
            {
                try
                {
                    string json = File.ReadAllText(fullPath);
                    Settings = Newtonsoft.Json.JsonConvert.DeserializeObject<NugetSetting>(json);
                    txtNugetServer.Text = Settings.Server;
                    txtApiKey.Text = Settings.ApiKey;
                }
                catch (Exception)
                {
                    File.Delete(fullPath);
                    Settings = new NugetSetting();
                }

            }
            else
            {
                Settings = new NugetSetting();
            }
        }
        private void SaveSettings()
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);

            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Settings);
            File.WriteAllText(fullPath, json);
        }
    }
}
