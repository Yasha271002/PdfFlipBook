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
using Core;
using IronPdf.Engines.WebKit.Settings;
using PdfFlipBook.Annotations;
using PdfFlipBook.Helper;
using PdfFlipBook.Models;
using PdfFlipBook.Utilities;
using GlobalSettings = PdfFlipBook.Helper.Singleton.GlobalSettings;

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

        public static readonly DependencyProperty ActualBackProperty = DependencyProperty.Register(
            "ActualBack", typeof(string), typeof(Razdel_Page), new PropertyMetadata(default(string)));

        public string ActualBack
        {
            get { return (string)GetValue(ActualBackProperty); }
            set { SetValue(ActualBackProperty, value); }
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

        private ScrollViewer _booksScrollViewer;
        private ScrollViewer _radioScrollViewer;

        public Razdel_Page(string razdel, List<BookPDF> actualBooks, SettingsModel settings)
        {
            InitializeComponent();
            ActualBack = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Background")[0];

            ActualRazdel = razdel;
            SettingsModel = settings;

            GetBooks(actualBooks, razdel);
            LearnCountBooks(actualBooks);

            IsFirstPage = true;
            IsLastPage = true;

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
        }

        private ICommand _moveUpCommand;
        private ICommand _moveDownCommand;

        public ICommand MoveUpCommand =>
            _moveUpCommand ??= new Command(c =>
            {
                MoveUp();
                GC.Collect();
            });

        public ICommand MoveDownCommand =>
            _moveDownCommand ??= new Command(c =>
            {
                MoveDown();
                GC.Collect();
            });

        private ICommand _bookCommand;

        public ICommand BookCommand =>
            _bookCommand ??= (_bookCommand = new Command(c =>
            {
                // App.CurrentApp.IsLoading = true;
                //int a = int.Parse(c.ToString())+1;
                var BookData = Tuple.Create(c.ToString(), SettingsModel);
                CommonCommands.NavigateCommand.Execute(BookData);
                GC.Collect();
            }));

        private ICommand _backCommand;

        public ICommand BackCommand =>
            _backCommand ??= (_backCommand = new Command(c =>
            {
                NavigationService?.Navigate(new Start_Page());
                GC.Collect();
            }));

        private ICommand _selectBookCommand;

        public ICommand SelectBookCommand =>
            _selectBookCommand ??= new Command(c =>
            {
                if (int.TryParse(c.ToString(), out int selectedIndex))
                {
                    SelectedBookIndex = selectedIndex - 1;
                    ScrollToSelectedElements();
                }
            });

        private void UpdatePageButtonsState()
        {
            IsFirstPage = SelectedBookIndex != 0;

            IsLastPage = SelectedBookIndex != CountBooks.Count - 1;
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

        public void GetBooks(List<BookPDF> actualBooks, string razdel)
        {
            ActualBooks = new ObservableCollection<BookPDF>();


            foreach (var actualBook in App.CurrentApp.ActualBooks)
            {
                if (actualBook.Icon.Source == null)
                    actualBook.Icon =
                        new DisposableImage(
                            Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" + actualBook.Title)[0]);
                ActualBooks.Add(actualBook);
            }
        }

        private void Razdel_Page_OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var actualBook in ActualBooks)
            {
                actualBook.Icon.Dispose();
            }

            foreach (var actualBook in App.CurrentApp.ActualBooks)
            {
                actualBook.Icon.Dispose();
            }

            ActualBooks.Clear();
            //App.CurrentApp.ActualBooks = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void LearnCountBooks(List<BookPDF> books)
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
        }

        private void Razdel_Page_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ActualBooks.Count != 0) return;
            foreach (var actualBook in App.CurrentApp.ActualBooks)
            {
                if (actualBook.Icon.Source == null)
                    actualBook.Icon =
                        new DisposableImage(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" +
                                                               actualBook.Title)[0]);
                ActualBooks.Add(actualBook);
            }
        }

        private void ScrollToSelectedElements()
        {
            if (_booksScrollViewer == null || _radioScrollViewer == null)
            {
                _booksScrollViewer = FindScrollViewer(BooksItemsControl);
                _radioScrollViewer = FindScrollViewer(RadioScrollViewer);
            }

            if (_booksScrollViewer == null) return;

            double booksItemHeight = 1553;
            int visibleItemsCount = (int)(_booksScrollViewer.ViewportHeight / booksItemHeight);

            if (SelectedBookIndex < 2)
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

            double radioItemHeight = 97;
            int visibleRadioItemsCount = (int)(_radioScrollViewer.ViewportHeight / radioItemHeight);

            if (SelectedBookIndex < 2)
            {
                _radioScrollViewer.ScrollToVerticalOffset(0);
            }
            else if (SelectedBookIndex >= CountBooks.Count - visibleRadioItemsCount / 2)
            {
                double offset = ((CountBooks.Count - visibleRadioItemsCount) * radioItemHeight) - radioItemHeight;
                _radioScrollViewer.ScrollToVerticalOffset(offset);
            }
            else
            {
                double offset = (SelectedBookIndex - visibleRadioItemsCount / 2) * radioItemHeight;
                _radioScrollViewer.ScrollToVerticalOffset(offset);
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
    }
}