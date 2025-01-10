using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PdfFlipBook.Helper;
using PdfFlipBook.Helper.Singleton;

namespace PdfFlipBook.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly JsonHelper _jsonHelper;

        public MainWindow()
        {
            InitializeComponent();

            _jsonHelper = new JsonHelper();
            GetBackground();

            App.CurrentApp.IsLoading = true;
            // UpdatePhotos();
            //Frame1.NavigationService?.Navigate(App.CurrentApp.SP, UriKind.Relative);
            NavigationManager.Frame1 = Frame1.NavigationService;
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            CommonCommands.NavigateCommand.Execute(PageTypes.StartPage);


            //this.Cursor = Cursors.None;
            Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = "/F /IM explorer.exe",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            process?.WaitForExit();
            Closing += (e, a) =>
            {
                Process.Start(System.IO.Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"));
            };

        }

        private void GetBackground()
        {
            var directoryPath = "Backgrounds";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var path = "background.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                _jsonHelper.WriteJsonToFile(path, Path.GetFullPath(Directory.GetFiles(directoryPath).FirstOrDefault()!), false);
            }
            else
            {
                var text = File.ReadAllText(path);
                if (text.Length == 0)
                    _jsonHelper.WriteJsonToFile(path, Path.GetFullPath(Directory.GetFiles(directoryPath).FirstOrDefault()!), false);
            }

            BackgroundSingleton.Instance.Background = Path.GetFullPath(_jsonHelper.ReadJsonFromFile<string>(path));
        }

        private void OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LicenseHelper.Utilities.LicenseManager.SetAppId(24);
            await LicenseHelper.Utilities.LicenseManager.CheckLicense();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
