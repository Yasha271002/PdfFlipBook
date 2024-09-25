using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using PdfFlipBook.Helper;
using PdfFlipBook.Models;
using PdfFlipBook.Views.Pages;


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
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();


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

        private SettingsModel _settings;

        public SettingsModel Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        private void OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }


        private void Frame1_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (Frame1.Content is Start_Page)
            {
                while (Frame1.NavigationService.CanGoBack)
                {
                    Frame1.NavigationService.RemoveBackEntry();
                }

            }
        }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
