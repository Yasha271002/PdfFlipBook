using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PdfFlipBook.Helper;
using PdfFlipBook.Helper.Singleton;
using PdfFlipBook.Models;
using PdfFlipBook.Properties;
using PdfFlipBook.Utilities;

namespace PdfFlipBook.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Razdel_Page.xaml
    /// </summary>
    public partial class Razdel_Page : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty ActualBooksProperty = DependencyProperty.Register(
            "ActualBooks", typeof(ObservableCollection<BookPDF>), typeof(Razdel_Page),
            new PropertyMetadata(default(ObservableCollection<BookPDF>)));

        public ObservableCollection<BookPDF> ActualBooks
        {
            get { return (ObservableCollection<BookPDF>)GetValue(ActualBooksProperty); }
            set { SetValue(ActualBooksProperty, value); }
        }

        public static readonly DependencyProperty ActualRazdelProperty = DependencyProperty.Register(
            "ActualRazdel", typeof(string), typeof(Razdel_Page), new PropertyMetadata(default(string)));

        public string ActualRazdel
        {
            get { return (string)GetValue(ActualRazdelProperty); }
            set { SetValue(ActualRazdelProperty, value); }
        }

        [CanBeNull] private ObservableCollection<GridSizeModel> _gridSizes;

        public ObservableCollection<GridSizeModel> GridSizes
        {
            get => _gridSizes ??= new ObservableCollection<GridSizeModel>();
            set
            {
                _gridSizes = value;
                OnPropertyChanged();
            }
        }

        private GridSizeModel _selectedGridSize;

        public GridSizeModel SelectedGridSize
        {
            get { return _selectedGridSize; }
            set
            {
                _selectedGridSize = value;
                OnPropertyChanged();
                UpdateItemTemplateSize(_selectedGridSize?.Size);
            }
        }

        private double _scrollViewerHeight;

        public double ScrollViewerHeights
        {
            get => _scrollViewerHeight;
            set
            {
                _scrollViewerHeight = value;
                OnPropertyChanged();
            }
        }

        private double _wrapPanelWidth;

        public double WrapPanelWidths
        {
            get => _wrapPanelWidth;
            set
            {
                _wrapPanelWidth = value;
                OnPropertyChanged();
            }
        }

        private bool _verticalScrollBarVisibility;

        public bool VerticalScrollBarVisibility
        {
            get => _verticalScrollBarVisibility;
            set
            {
                _verticalScrollBarVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _isFirstPage;

        public bool IsFirstPage
        {
            get => _isFirstPage;
            set
            {
                _isFirstPage = value;
                OnPropertyChanged();
            }
        }

        private bool _isLastPage;

        public bool IsLastPage
        {
            get => _isLastPage;
            set
            {
                _isLastPage = value;
                OnPropertyChanged();
            }
        }

        private int _selectedBookIndex;

        public int SelectedBookIndex
        {
            get => _selectedBookIndex;
            set
            {
                if (_selectedBookIndex != value)
                {
                    _selectedBookIndex = value;
                    OnPropertyChanged();
                    UpdatePageButtonsState();

                    ScrollToSelectedElements();
                }
            }
        }


        private List<CountBooksModel> _countBooks;

        public List<CountBooksModel> CountBooks
        {
            get => _countBooks;
            set
            {
                _countBooks = value;
                OnPropertyChanged();
            }
        }

        private SettingsModel _settings;

        public SettingsModel SettingsModel
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        private bool _isDotsVisibility;

        public bool IsDotsVisibility
        {
            get => _isDotsVisibility;
            set
            {
                _isDotsVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _isCheckedRadio;

        public bool IsCheckedRadio
        {
            get => _isCheckedRadio;
            set
            {
                _isCheckedRadio = value;
                OnPropertyChanged();
            }
        }

        private string _isLastBookCount;

        public string IsLastBookCount
        {
            get => _isLastBookCount;
            set
            {
                _isLastBookCount = value;
                OnPropertyChanged();
            }
        }

        private double _itemsControlHeight;

        public double ItemsControlHeight
        {
            get => _itemsControlHeight;
            set
            {
                _itemsControlHeight = value;
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

        private ScrollViewer _booksScrollViewer;
        private ScrollViewer _radioScrollViewer;

        private ObservableCollection<BookPDF> _allBooks;
        private const int _initialLoadCount = 20;
        private const int _incrementLoadCount = 10;
        private bool _isLoadingMoreBooks = false;
        private AudioHelper _audioHelper;

        public Razdel_Page(string razdel, ObservableCollection<BookPDF> actualBooks, SettingsModel settings,
            BookFolder selectRazdel)
        {
            InitializeComponent();

            ActualBooks = actualBooks;
            SelectRazdel = selectRazdel;
            ActualRazdel = razdel;
            SettingsModel = settings;

            LearnCountBooks(actualBooks);

            UpdatePageButtonsState();

            _audioHelper = new AudioHelper(SelectRazdel.Sound, SettingsModel.Volume);

            GridSizes = new ObservableCollection<GridSizeModel>
            {
                new GridSizeModel { Size = "1x1" },
                new GridSizeModel { Size = "2x2" },
                new GridSizeModel { Size = "3x2" },
                new GridSizeModel { Size = "3x3" },
                new GridSizeModel { Size = "6x2" },
                new GridSizeModel { Size = "7x3" }
            };
            SelectedGridSize = GridSizes.FirstOrDefault();
            UpdatePageButtonsState();
            InitializeAudio();
        }

        private ICommand _moveUpCommand;
        private ICommand _moveDownCommand;

        public ICommand MoveUpCommand =>
            _moveUpCommand ??= new Command(c => { MoveUp(); });

        public ICommand MoveDownCommand =>
            _moveDownCommand ??= new Command(c => { MoveDown(); });

        private ICommand _bookCommand;

        public ICommand BookCommand =>
            _bookCommand ??= (_bookCommand = new Command(c =>
            {
                // App.CurrentApp.IsLoading = true;
                //int a = int.Parse(c.ToString())+1;
                var BookData = Tuple.Create(c.ToString(), SettingsModel, SelectRazdel);
                CommonCommands.NavigateCommand.Execute(BookData);
            }));

        private ICommand _backCommand;

        public ICommand BackCommand =>
            _backCommand ??= (_backCommand = new Command(c => { NavigationService?.Navigate(new Start_Page()); }));

        private ICommand _selectBookCommand;

        public ICommand SelectBookCommand =>
            _selectBookCommand ??= new Command(c =>
            {
                if (!int.TryParse(c.ToString(), out var selectedIndex)) return;

                SelectedBookIndex = selectedIndex - 1;
                ScrollToSelectedElements();
            });


        private void UpdatePageButtonsState()
        {
            if (CountBooks.Count == 0)
            {
                IsLastPage = false;
            }
            else
            {
                IsFirstPage = false || SelectedBookIndex != 0;
                IsLastPage = SelectedBookIndex != CountBooks.Count - 1;
            }
        }

        private void MoveUp()
        {
            if (SelectedBookIndex > 0)
                SelectedBookIndex--;
        }

        private void MoveDown()
        {
            if (SelectedBookIndex < CountBooks.Count - 1)
                SelectedBookIndex++;
        }

        private void LearnCountBooks(ObservableCollection<BookPDF> books)
        {
            CountBooks = new List<CountBooksModel>();
            var index = 1;
            for (int i = 0; i < books.Count; i++)
            {
                CountBooks.Add(new CountBooksModel
                {
                    Count = index.ToString()
                });
                index++;
            }

            if (CountBooks.Count > 5)
            {
                ItemsControlHeight = 282.0;
                IsDotsVisibility = true;
            }
            else
            {
                ItemsControlHeight = 482.0;
                IsDotsVisibility = false;
            }

            IsLastBookCount = CountBooks.Count.ToString();
        }


        private void Razdel_Page_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ActualBooks.Count != 0) return;
            foreach (var actualBook in App.CurrentApp.ActualBooks)
            {
                actualBook.Icon ??= Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" +
                                                       actualBook.Title)[0];
                ActualBooks.Add(actualBook);
            }
            InitializeAudio();
        }

        private void InitializeAudio()
        {
            if (_audioHelper.IsPlaying)
                return;
            _audioHelper = new AudioHelper(SelectRazdel.Sound, SettingsModel.Volume);
            PlaySound();
        }

        private void Razdel_Page_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ActualBooks.Clear();
            _audioHelper.Exit();
        }

        private void ScrollToSelectedElements()
        {
            if (_booksScrollViewer == null || _radioScrollViewer == null)
            {
                _booksScrollViewer = FindScrollViewer(BooksItemsControl);
                _radioScrollViewer = FindScrollViewer(RadioScrollViewer);
            }

            if (_booksScrollViewer == null) return;

            IsCheckedRadio = SelectedBookIndex == Convert.ToInt32(IsLastBookCount);

            double booksItemHeight = 1553.0;
            int visibleItemsCount = (int)(_booksScrollViewer.ViewportHeight / booksItemHeight);

            if (SelectedBookIndex < 1)
            {
                _booksScrollViewer.ScrollToVerticalOffset(0);
            }
            else if (SelectedBookIndex >= CountBooks.Count - visibleItemsCount / 2)
            {
                double offset = (CountBooks.Count - visibleItemsCount) * booksItemHeight;
                _booksScrollViewer.ScrollToVerticalOffset(offset);
            }
            else
            {
                double offset = (SelectedBookIndex - visibleItemsCount / 2) * booksItemHeight;
                _booksScrollViewer.ScrollToVerticalOffset(offset);
            }

            if (_radioScrollViewer == null) return;

            double radioItemHeight = 97.0;
            int visibleRadioItemsCount = (int)(_radioScrollViewer.ViewportHeight / radioItemHeight);

            if (SelectedBookIndex < 5 && CountBooks.Count < 5)
            {
                _radioScrollViewer.ScrollToVerticalOffset(0);
                ItemsControlHeight = 482.0;
                IsDotsVisibility = false;
            }
            else if (SelectedBookIndex >= (CountBooks.Count - 3) - visibleRadioItemsCount / 2)
            {
                ItemsControlHeight = 482.0;
                double offset = ((CountBooks.Count - visibleRadioItemsCount) * radioItemHeight) - radioItemHeight;
                IsDotsVisibility = false;
                _radioScrollViewer.ScrollToVerticalOffset(offset);
            }
            else
            {
                double offset = (SelectedBookIndex - visibleRadioItemsCount / 2.0) * radioItemHeight;

                _radioScrollViewer.ScrollToVerticalOffset(offset);
                ItemsControlHeight = 282.0;
                IsDotsVisibility = true;
            }
        }

        private ScrollViewer FindScrollViewer(DependencyObject parent)
        {
            if (parent is ScrollViewer)
                return parent as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var result = FindScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void UpdateItemTemplateSize(string gridSize)
        {
            var BorderWidth = 0.0;
            var BorderHeight = 0.0;

            var ImageHeight = 0.0;
            var ImageWidth = 0.0;

            var WrapPanelWidth = 0.0;

            var ScrollViewerHeight = 0.0;

            switch (gridSize)
            {
                case "1x1":
                    BorderWidth = 1270.0;
                    BorderHeight = 1513.0;

                    ImageWidth = 1270.0;
                    ImageHeight = 1270.0;

                    WrapPanelWidth = 1270;

                    ScrollViewerHeight = 1513.0;

                    VerticalScrollBarVisibility = false;
                    break;
                case "2x2":
                    BorderWidth = 658.0;
                    BorderHeight = 919.0;

                    ImageWidth = 600.0;
                    ImageHeight = 600.0;

                    WrapPanelWidth = 1474.0;

                    ScrollViewerHeight = 1860.0;

                    VerticalScrollBarVisibility = true;
                    break;
                case "3x3":
                    BorderWidth = 500.0;
                    BorderHeight = 591.0;

                    ImageWidth = 454.0;
                    ImageHeight = 390.0;

                    WrapPanelWidth = 1950.0;

                    ScrollViewerHeight = 1860.0;

                    VerticalScrollBarVisibility = true;
                    break;
                case "3x2":
                    BorderWidth = 658.0;
                    BorderHeight = 930.0;

                    ImageWidth = 600.0;
                    ImageHeight = 600.0;

                    WrapPanelWidth = 2450.0;

                    ScrollViewerHeight = 1980.0;

                    VerticalScrollBarVisibility = true;
                    break;
                case "6x2":
                    BorderWidth = 539.0;
                    BorderHeight = 740.0;

                    ImageWidth = 539.0;
                    ImageHeight = 539.0;

                    WrapPanelWidth = 3526.0;

                    ScrollViewerHeight = 1540;

                    VerticalScrollBarVisibility = true;
                    break;
                case "7x3":
                    BorderWidth = 454.0;
                    BorderHeight = 580.0;

                    ImageWidth = 454.0;
                    ImageHeight = 390.0;

                    WrapPanelWidth = 3526.0;

                    ScrollViewerHeight = 1860.0;

                    VerticalScrollBarVisibility = true;
                    break;
            }

            WrapPanelWidths = WrapPanelWidth;
            ScrollViewerHeights = ScrollViewerHeight;

            foreach (var item in ActualBooks)
            {
                item.BorderWidth = BorderWidth;
                item.BorderHeight = BorderHeight;

                item.ImageHeight = ImageHeight;
                item.ImageWidth = ImageWidth;

                item.WrapPanelWidth = WrapPanelWidth;
                item.ScrollViewerHeight = ScrollViewerHeight;
            }
        }

        private void Razdel_Page_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        #region Sound

        private void PlaySound()
        {
            if (_audioHelper.IsPlaying)
                _audioHelper.Stop();

            _audioHelper.InfinityPlay();
        }

        private void StopSound()
        {
            if (!_audioHelper.IsPlaying)
                return;

            _audioHelper.Stop();
        }

        #endregion
    }
}