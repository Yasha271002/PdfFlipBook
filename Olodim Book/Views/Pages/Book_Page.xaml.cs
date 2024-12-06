using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using PdfFlipBook.Helper;
using System.Windows.Threading;
using PdfFlipBook.Utilities;
using Path = System.IO.Path;
using PdfFlipBook.Models;
using PdfFlipBook.Helper.Singleton;

namespace PdfFlipBook.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Book_Page.xaml
    /// </summary>
    public partial class Book_Page : Page, INotifyPropertyChanged
    {
        #region DependencyProperty

        public static readonly DependencyProperty AllPagesProperty = DependencyProperty.Register(
            "AllPages", typeof(List<string>), typeof(Book_Page),
            new PropertyMetadata(default(List<string>)));

        public List<string> AllPages
        {
            get { return (List<string>)GetValue(AllPagesProperty); }
            set { SetValue(AllPagesProperty, value); }
        }

        private void Book_Page_OnLoaded(object sender, RoutedEventArgs e)
        {
            // App.CurrentApp.IsLoading = true;
            _inactivityHelper.OnInactivity += OnInactivityDetected;
        }

        public static readonly DependencyProperty AllPhotosProperty = DependencyProperty.Register(
            "AllPhotos", typeof(List<string>), typeof(Book_Page), new PropertyMetadata(default(List<string>)));

        public List<string> AllPhotos
        {
            get { return (List<string>)GetValue(AllPhotosProperty); }
            set { SetValue(AllPhotosProperty, value); }
        }

        public static readonly DependencyProperty StartPointXProperty = DependencyProperty.Register(
            "StartPointX", typeof(double), typeof(Book_Page), new PropertyMetadata(default(double)));

        public double StartPointX
        {
            get { return (double)GetValue(StartPointXProperty); }
            set { SetValue(StartPointXProperty, value); }
        }

        public static readonly DependencyProperty BookIndexProperty = DependencyProperty.Register(
            "BookIndex", typeof(int), typeof(Book_Page), new PropertyMetadata(default(int)));

        public int BookIndex
        {
            get { return (int)GetValue(BookIndexProperty); }
            set { SetValue(BookIndexProperty, value); }
        }

        public static readonly DependencyProperty BookTitleProperty = DependencyProperty.Register(
            "BookTitle", typeof(string), typeof(Book_Page), new PropertyMetadata(default(string)));

        public string BookTitle
        {
            get { return (string)GetValue(BookTitleProperty); }
            set { SetValue(BookTitleProperty, value); }
        }

        #endregion

        #region INotifyPropertyChangedRegion

        private DispatcherTimer _pageFlipTimer;
        private readonly BaseInactivityHelper _inactivityHelper;

        private int _currentPageNumber;

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set
            {
                _currentPageNumber = value;
                OnPropertyChanged();
            }
        }

        private string? _pageNumber;

        public string? PageNumber
        {
            get => _pageNumber;
            set
            {
                _pageNumber = value;
                OnPropertyChanged();
            }
        }

        private int _indexBook;

        public int IndexBook
        {
            get => _indexBook;
            set
            {
                _indexBook = value;
                OnPropertyChanged();
            }
        }

        private int _pageIndex;

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                _pageIndex = value;
                OnPropertyChanged();

                CurrentPageNumber = PageIndex;
                GetPageNumber();
            }
        }

        private SettingsModel _settingsModel;

        public SettingsModel SettingsModel
        {
            get => _settingsModel;
            set
            {
                _settingsModel = value;
                OnPropertyChanged();
            }
        }

        private BookFolder _selectRazdel;

        public BookFolder SelectRazdel
        {
            get => _selectRazdel;
            set
            {
                _selectRazdel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region ICommandRegion

        private ICommand _backCommand;

        public ICommand BackCommand =>
            _backCommand ??= (_backCommand = new Command(c =>
            {
                _pageFlipTimer.Stop();
                _inactivityHelper.OnInactivity -= OnInactivityDetected;

                NavigationService?.GoBack();
            }));

        private ICommand _insertPageCommand;

        public ICommand InsertPageCommand =>
            _insertPageCommand ??= (_insertPageCommand = new Command(c =>
            {
                InsertGrid.Visibility = Visibility.Visible;
            }));

        private ICommand _closeInsertCommand;

        public ICommand CloseInsertCommand =>
            _closeInsertCommand ??= (_closeInsertCommand = new Command(c =>
            {
                InsertGrid.Visibility = Visibility.Collapsed;
                KK.Text = string.Empty;
            }));

        private ICommand _deleteCommand;

        public ICommand DeleteCommand =>
            _deleteCommand ??= (_deleteCommand = new Command(c =>
            {
                if (KK.Text == string.Empty) return;
                string qweq = KK.Text.Remove(KK.Text.Length - 1);
                KK.Text = qweq;
            }));


        private ICommand _toPageCommand;

        public ICommand ToPageCommand =>
            _toPageCommand ?? (_toPageCommand = new Command(c =>
            {
                if (KK.Text == "" && AllPages.Count > int.Parse(KK.Text)) return;
                int page = int.Parse(KK.Text);

                AllPages = new List<string>();

                AllPhotos = new List<string>(Directory
                    .GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" + BookTitle).ToList()
                    .OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x))));
                foreach (var s in AllPhotos)
                {
                    AllPages.Add(s);
                }

                var ostatok = page % 2;
                if (ostatok == 1)
                    page--;

                Book.CurrentSheetIndex = page / 2;
                int index = Book.CurrentSheetIndex;
                try
                {
                    ReloadPage(page);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                CloseInsertCommand.Execute(null);

                PageIndex = page;
                GetPageNumber();
            }));

        private ICommand _switchPageCommand;

        public ICommand SwitchPageCommand => _switchPageCommand ??= new Command(f =>
        {
            if (f is not string type)
                return;
            FlipPage(type);
        });

        private ICommand _buttonClickCommand;

        public ICommand ButtonClickCommand => _buttonClickCommand ??= new Command(f =>
        {
            if (f is not string text)
                return;

            KK.Text += text;
        });

        #endregion

        private double _startPoint;
        private double _endPoint;

        #region EventRegion
        private void Book_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPointX = e.GetPosition(this).X;
            _startPoint = e.GetPosition(this).X;
        }

        private void OnInactivityDetected(int inactivityTime)
        {
            _pageFlipTimer.Start();
        }

        private void OnPageFlipTimerTick(object sender, EventArgs e)
        {
            FlipPage("+");
        }

        private void Book_Page_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _audioHelper.Exit();
            _pageFlipTimer.Stop();
            _inactivityHelper.OnInactivity -= OnInactivityDetected;
        }

        private void Book_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _endPoint = e.GetPosition(this).X;

            if (_startPoint >= 2300 && _endPoint < 1920)
            {
                GetPageNumber();
            }

            if (_startPoint < 1500 && _endPoint >1920)
            {
                GetPageNumber();
            }
            

            PlaySound();
        }

        private void Book_Page_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _pageFlipTimer.Stop();
            _inactivityHelper.OnInactivity -= OnInactivityDetected;

            _inactivityHelper.OnInactivity += OnInactivityDetected;
        }

        #endregion

        #region FlipPageMethods

        private void GetPageNumber()
        {
            var currentPage = Book.CurrentSheetIndex * 2;
            var totalPages = AllPages.Count;

            PageNumber = currentPage == totalPages 
                ? $"{currentPage - 1}-{currentPage} из {totalPages}" 
                : $"{currentPage}-{currentPage + 1} из {totalPages}";
        }

        private void FlipPage(string type)
        {
            switch (type)
            {
                case "+":
                    PageIndex = Book.CurrentSheetIndex + 2;
                    type = Flip(type);
                    //if (type == "stop")
                    //    return;
                    PlaySound();
                    Book.AnimateToNextPage(false, 1000);
                    GetPageNumber();
                    break;
                case "-":
                    PageIndex = Book.CurrentSheetIndex - 2;
                    type = Flip(type);
                    //if (type == "stop")
                    //    return;
                    PlaySound();
                    Book.AnimateToPreviousPage(false, 1000);
                    GetPageNumber();
                    break;
                default:
                    return;
            }
        }

        private string Flip(string type)
        {
            var halfPhotosCount = (int)Math.Ceiling(AllPhotos.Count / 2.0);
            IndexBook = GlobalSettings.Instance.Books.IndexOf(
                GlobalSettings.Instance.Books.FirstOrDefault(f => f.Title == BookTitle));

            if (PageIndex >= halfPhotosCount)
            {
                if (SettingsModel.Repeat)
                {
                    Book.CurrentSheetIndex = 0;
                    PageIndex = 0;
                }
                else
                {
                    if (GlobalSettings.Instance.Books.Count == 0) return "stop";
                    if (IndexBook >= GlobalSettings.Instance.Books.Count)
                        IndexBook = 0;

                    var newTitle = GlobalSettings.Instance.Books[IndexBook].Title;
                    ReloadBookPages(newTitle);

                    IndexBook++;
                    PageIndex = 0;
                    return type;
                }
            }
            else if (PageIndex < 0)
            {
                PageIndex = 0;
                type = "stop";
            }

            GetPageNumber();
            return type;
        }
        #endregion

        #region Sound

        private AudioHelper _audioHelper;

        private void PlaySound()
        {
            if (_audioHelper.IsPlaying)
                _audioHelper.Stop();

            _audioHelper.Play();
        }

        private void StopSound()
        {
            if (!_audioHelper.IsPlaying)
                return;

            _audioHelper.Stop();
        }

        #endregion

        public Book_Page(string bookTitle, SettingsModel settings, BookFolder selectRazdelBookFolder)
        {
            InitializeComponent();
            SelectRazdel = selectRazdelBookFolder;
            BookTitle = bookTitle;

            SettingsModel = settings;
            AllPages = new List<string>();

            AllPhotos = new List<string>(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" + bookTitle)
                .ToList().OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x))));
            foreach (var s in AllPhotos)
            {
                AllPages.Add(s);
            }

            _pageFlipTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(Convert.ToDouble(settings.IntervalSwitchPage))
            };

            _pageFlipTimer.Tick += OnPageFlipTimerTick;

            _inactivityHelper = new BaseInactivityHelper(Convert.ToInt32(settings.InactivityTime));
            _inactivityHelper.OnInactivity += OnInactivityDetected;

            CurrentPageNumber = 0;
            PageIndex = 0;

            GetPageNumber();

            _audioHelper = new AudioHelper(settings.SwitchSoundPath, settings.Volume);
        }

        private void ReloadBookPages(string newBookTitle)
        {
            if (AllPages != null)
            {
                AllPages.Clear();
            }

            AllPhotos?.Clear();

            BookTitle = newBookTitle;

            AllPhotos = new List<string>(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" + newBookTitle)
                .ToList()
                .OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x))));

            foreach (var photoPath in AllPhotos)
            {
                AllPages.Add(photoPath);
            }
        }
        private void ReloadPage(int page)
        {
            if (page < 0 || page >= AllPages.Count / 2)
            {
                return;
            }

            var index = page / 2;
            if (index * 2 < 0 || index * 2 >= AllPages.Count) return;
            try
            {
                AllPages[index * 2] = AllPhotos[index * 2].ToString();
                AllPages[index * 2 + 1] = AllPhotos[index * 2 + 1].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}