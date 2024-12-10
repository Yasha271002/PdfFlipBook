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
using WPFMitsuControls;

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
                int index = Book.CurrentSheetIndex*2;
                try
                {
                    ReloadPage(page);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                CloseInsertCommand.Execute(null);

                PageNumber = $"{index}-{index + 1} из {AllPages.Count}";

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

        private double _endPoint;

        #region EventRegion

        private void Book_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);

            StartPointX = position.X;
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
            if (StartPointX >= 2300 && _endPoint < 1920)
            {
                GetPageNumber("+");
            }

            if (StartPointX < 1500 && _endPoint > 1920)
            {
                GetPageNumber("-");
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

        private void GetPageNumber(string type)
        {
            var currentPage = 0;

            currentPage = type switch
            {
                "+" => Book.CurrentSheetIndex * 2 + 2,
                "-" => Book.CurrentSheetIndex * 2 - 2,
                _ => Book.CurrentSheetIndex
            };

            if (currentPage < 0)
            {
                currentPage = 0;
            }
            else if (currentPage > AllPages.Count)
            {
                currentPage = AllPages.Count;
            }

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
                    Flip(type);
                    PlaySound();
                    break;
                case "-":
                    Flip(type);
                    PlaySound();
                    break;
                default:
                    return;
            }
        }

        private void Flip(string type)
        {
            var index = Book.CurrentSheetIndex + 1;
            var halfPhotosCount = (int)Math.Ceiling(AllPhotos.Count / 2.0);
            IndexBook = GlobalSettings.Instance.Books.IndexOf(
                GlobalSettings.Instance.Books.FirstOrDefault(f => f.Title == BookTitle));

            if (index >= halfPhotosCount)
            {
                if (SettingsModel.Repeat)
                {
                    Book.CurrentSheetIndex = 0;
                    PageNumber = $"0-1 из {AllPages.Count.ToString()}";
                }
                else
                {
                    if (GlobalSettings.Instance.Books.Count == 0) return;
                    IndexBook++;
                    if (IndexBook >= GlobalSettings.Instance.Books.Count)
                        IndexBook = 0;

                    var newTitle = GlobalSettings.Instance.Books[IndexBook].Title;
                    ReloadBookPages(newTitle);
                }
            }
            else if (Book.CurrentSheetIndex < 0)
            {
                Book.CurrentSheetIndex = 0;
            }
            else
            {
                switch (type)
                {
                    case "+":
                        Book.AnimateToNextPage(false, 1000);
                        GetPageNumber("+");
                        break;
                    case "-":
                        Book.AnimateToPreviousPage(false, 1000);
                        GetPageNumber("-");
                        break;
                }
            }
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

            PageNumber = $"0-1 из {AllPages.Count.ToString()}";

            _audioHelper = new AudioHelper(settings.SwitchSoundPath, settings.Volume);
        }

        private void ReloadBookPages(string newBookTitle)
        {
            if (AllPages != null)
            {
                AllPages.Clear();
            }

            if (AllPhotos != null)
            {
                AllPhotos.Clear();
            }


            BookTitle = newBookTitle;

            try
            {
                AllPhotos = new List<string>(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" + newBookTitle)
                    .ToList()
                    .OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x))));

                foreach (var photoPath in AllPhotos)
                {
                    AllPages.Add(photoPath);
                }

                Book.CurrentSheetIndex = 0;
                PageNumber = $"0-1 из {AllPages.Count.ToString()}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pages for book {newBookTitle}: {ex.Message}");
            }
        }

        private void ReloadPage(int page)
        {
            if (page < 0 || page >= AllPages.Count)
            {
                return;
            }

            var index = page / 2;
            if (index * 2 < 0 || index * 2 >= AllPages.Count) return;

            try
            {
                AllPages[index * 2] = AllPhotos[index * 2];
                if (index * 2 + 1 < AllPages.Count)
                {
                    AllPages[index * 2 + 1] = AllPhotos[index * 2 + 1];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reloading page {page}: {ex.Message}");
            }
        }
    }
}