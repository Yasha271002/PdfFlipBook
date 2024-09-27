using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PdfFlipBook.Helper;


namespace PdfFlipBook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            ActualBack = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Background")[0];

            App.CurrentApp.IsLoading = true;
            // UpdatePhotos();
            Frame1.NavigationService?.Navigate(App.CurrentApp.SP, UriKind.Relative);
            NavigationManager.Frame1 = Frame1.NavigationService;


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

        public static readonly DependencyProperty ActualBackProperty = DependencyProperty.Register(
            "ActualBack", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        public string ActualBack
        {
            get { return (string)GetValue(ActualBackProperty); }
            set { SetValue(ActualBackProperty, value); }
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
