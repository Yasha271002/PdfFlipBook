﻿using System;
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

        private DispatcherTimer _pageFlipTimer;
        private readonly BaseInactivityHelper _inactivityHelper;

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

        public Book_Page(string bookTitle, SettingsModel settings)
        {
            InitializeComponent();

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
            GetPageNumber();
        }

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
                onScreenKeyboard.Text = string.Empty;
            }));

        private ICommand _deleteCommand;

        public ICommand DeleteCommand =>
            _deleteCommand ??= (_deleteCommand = new Command(c =>
            {
                if (KK.Text == string.Empty) return;
                string qweq = KK.Text.Remove(KK.Text.Length - 1);
                KK.Text = qweq;
                onScreenKeyboard.Text = qweq;
            }));


        private ICommand _toPageCommand;


        private void Book_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPointX = e.GetPosition(this).X;
        }

        private void OnInactivityDetected(int inactivityTime)
        {
            _pageFlipTimer.Start();
        }

        private void OnPageFlipTimerTick(object sender, EventArgs e)
        {
            FlipPage();
        }

        private void FlipPage()
        {
            PageIndex = Book.CurrentSheetIndex + 1;
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
                    if(GlobalSettings.Instance.Books.Count == 0) return;
                    if (IndexBook >= GlobalSettings.Instance.Books.Count)
                        IndexBook = 0;

                    var newTitle = GlobalSettings.Instance.Books[IndexBook].Title;
                    ReloadBookPages(newTitle);

                    IndexBook++;
                    PageIndex = 0;
                    return;
                }
            }

            GetPageNumber();
            Book.AnimateToNextPage(false, 1000);
        }

        private void Book_Page_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _pageFlipTimer.Stop();
            _inactivityHelper.OnInactivity -= OnInactivityDetected;
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

        public ICommand ToPageCommand =>
            _toPageCommand ?? (_toPageCommand = new Command(c =>
            {
                if (KK.Text == "") return;
                int page = int.Parse(KK.Text);
                if (page > AllPages.Count)
                {
                    var animation = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(1.3),
                        AutoReverse = true
                    };
                    TB.BeginAnimation(UIElement.OpacityProperty, animation);
                }
                else
                {
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

                    PageIndex = page;

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
                }
            }));

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


        private void Book_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int index = Book.CurrentSheetIndex;
            // MessageBox.Show(index.ToString());
            var xPosition = e.GetPosition(this).X;
            if (AllPages.Count <= 30) return;
            if (xPosition < 1920)
            {
                if (!(StartPointX > 1920)) return;
                try
                {
                    AllPages.RemoveAt(index * 2 + 1);
                    AllPages.Insert(index * 2 + 1, AllPhotos[index * 2 + 1]);
                    AllPages.RemoveAt(index * 2 + 2);
                    AllPages.Insert(index * 2 + 2, AllPhotos[index * 2 + 2]);
                    AllPages.RemoveAt(index * 2 + 3);
                    AllPages.Insert(index * 2 + 3, AllPhotos[index * 2 + 3]);
                    AllPages.RemoveAt(index * 2 + 4);
                    AllPages.Insert(index * 2 + 4, AllPhotos[index * 2 + 4]);
                }
                catch (Exception exception)
                {
                }
            }

            else
            {
                if (!(StartPointX < 1920)) return;
                try
                {
                    AllPages.RemoveAt(index * 2 - 4);
                    AllPages.Insert(index * 2 - 4, AllPhotos[index * 2 - 4]);
                    AllPages.RemoveAt(index * 2 - 5);
                    AllPages.Insert(index * 2 - 5, AllPhotos[index * 2 - 5]);
                    AllPages.RemoveAt(index * 2 - 6);
                    AllPages.Insert(index * 2 - 6, AllPhotos[index * 2 - 6]);
                    AllPages.RemoveAt(index * 2 - 7);
                    AllPages.Insert(index * 2 - 7, AllPhotos[index * 2 - 7]);
                }
                catch (Exception exception)
                {
                    
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Book_Page_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _inactivityHelper.OnInactivity -= OnInactivityDetected;
            _inactivityHelper.OnInactivity += OnInactivityDetected;
        }

        #region CountPage

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

        private ICommand _switchPageCommand;
        public ICommand SwitchPageCommand => _switchPageCommand ??= new Command(f =>
        {
            if (f is not string type)
                return;

            switch (type)
            {
                case "+":
                    SwitchPage(type);
                    break;
                case "-":
                    SwitchPage(type);
                    break;
            }
        });

        private void SwitchPage(string type)
        {
            PageIndex = type switch
            {
                "+" => Book.CurrentSheetIndex + 1,
                "-" => Book.CurrentSheetIndex - 1,
                _ => PageIndex
            };

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
                    if (GlobalSettings.Instance.Books.Count == 0) return;
                    if (IndexBook >= GlobalSettings.Instance.Books.Count)
                        IndexBook = 0;

                    var newTitle = GlobalSettings.Instance.Books[IndexBook].Title;
                    ReloadBookPages(newTitle);

                    IndexBook++;
                    PageIndex = 0;
                    return;
                }
            }

            GetPageNumber();
            Book.AnimateToNextPage(false, 1000);
        }

        private void GetPageNumber()
        {
            PageNumber = $"{CurrentPageNumber}-{++CurrentPageNumber} из {AllPages.Count}";
        }


        #endregion
    }
}