using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iDiTect.Converter;
using PdfFlipBook.Helper;
using PdfFlipBook.Models;
using PdfFlipBook.Utilities;
using PdfFlipBook.Views;
using PdfFlipBook.Views.Pages;
using WPFMitsuControls;
using Image = System.Drawing.Image;
using Path = System.IO.Path;


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

            App.CurrentApp.IsLoading = true;
            // UpdatePhotos();
            Frame1.NavigationService?.Navigate(App.CurrentApp.SP, UriKind.Relative);
            NavigationManager.Frame1 = Frame1.NavigationService;
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();


            //this.Cursor = Cursors.None;
            //Process process = Process.Start(new ProcessStartInfo
            //{
            //    FileName = "taskkill",
            //    Arguments = "/F /IM explorer.exe",
            //    CreateNoWindow = true,
            //    UseShellExecute = false,
            //    WindowStyle = ProcessWindowStyle.Hidden
            //});
            //process?.WaitForExit();
            //Closing += (e, a) =>
            //{
            //    Process.Start(System.IO.Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"));
            //};

        }

        public static readonly DependencyProperty AllPagesProperty = DependencyProperty.Register(
            "AllPages", typeof(ObservableCollection<DisposableImage>), typeof(MainWindow), new PropertyMetadata(default(ObservableCollection<DisposableImage>)));

        public ObservableCollection<DisposableImage> AllPages
        {
            get { return (ObservableCollection<DisposableImage>)GetValue(AllPagesProperty); }
            set { SetValue(AllPagesProperty, value); }
        }

        public static readonly DependencyProperty PagesProperty = DependencyProperty.Register(
            "Pages", typeof(List<string>), typeof(MainWindow), new PropertyMetadata(default(List<string>)));

        public List<string> Pages
        {
            get { return (List<string>)GetValue(PagesProperty); }
            set { SetValue(PagesProperty, value); }
        }

        public static readonly DependencyProperty AllPhotosProperty = DependencyProperty.Register(
            "AllPhotos", typeof(List<string>), typeof(MainWindow), new PropertyMetadata(default(List<string>)));

        public List<string> AllPhotos
        {
            get { return (List<string>) GetValue(AllPhotosProperty); }
            set { SetValue(AllPhotosProperty, value); }
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

        public static byte[] converterDemo(System.Drawing.Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
            
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public BitmapImage ImageFromBuffer(Byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        private Bitmap SourceToBitmap(BitmapSource source)
        {
            Bitmap bmp;
            using (MemoryStream ms = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(ms);
                bmp = new Bitmap(ms);
            }
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            return bmp;
            
        }

        //private void Book_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    MessageBox.Show(Book.CurrentSheetIndex.ToString());

        //    AllPages[Book.CurrentSheetIndex].Dispose();
        //}

        private void Frame1_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            var ta = new DoubleAnimation();
            ta.Duration = TimeSpan.FromSeconds(0.6);
            ta.DecelerationRatio = 0.4;
            ta.To = 1;
            if (e.NavigationMode == NavigationMode.New)
            {
                ta.From = 0.3;
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                ta.From = 0;
            }
            (e.Content as Page).BeginAnimation(OpacityProperty, ta);
        }
        private void Frame1_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (Frame1.Content is Start_Page)
            {
                while (Frame1.NavigationService.CanGoBack)
                {
                    Frame1.NavigationService.RemoveBackEntry();
                }
                GC.Collect();

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

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
