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
using PdfFlipBook.Annotations;
using PdfFlipBook.Helper;
using PdfFlipBook.Models;
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

        public ObservableCollection<GridSizeModel> GridSizes { get; set; }

        private GridSizeModel _selectedGridSize;

        public GridSizeModel SelectedGridSize
        {
            get { return _selectedGridSize; }
            set { _selectedGridSize = value; OnPropertyChanged();
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

        public Razdel_Page(string razdel, List<BookPDF> actualBooks)
        {
            InitializeComponent();
            ActualBack = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Background")[0];

            ActualRazdel = razdel;

            GetBooks(actualBooks, razdel);

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

            ComboBoxGridSize.ItemsSource = GridSizes;
        }

        private ICommand _bookCommand;

        public ICommand BookCommand =>
            _bookCommand ??= (_bookCommand = new Command(c =>
            {
                // App.CurrentApp.IsLoading = true;
                //int a = int.Parse(c.ToString())+1;
                NavigationService?.Navigate(new Book_Page(c.ToString()));

                GC.Collect();
            }));

        
        private ICommand _backCommand;

        public ICommand BackCommand =>
            _backCommand ??= (_backCommand = new Command(c =>
            {
                NavigationService?.Navigate(new Start_Page());
                GC.Collect();
            }));
        
        

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
                    BorderWidth = 3526.0;
                    BorderHeight = 1513.0;

                    ImageWidth = 1270.0;
                    ImageHeight = 1270.0;

                    WrapPanelWidth = 3526.0;

                    ScrollViewerHeight = 1513.0;
                    break;
                case "2x2":
                    BorderWidth = 658.0;
                    BorderHeight = 919.0;

                    ImageWidth = 600.0;
                    ImageHeight = 600.0;

                    WrapPanelWidth = 1474.0;

                    ScrollViewerHeight = 1860.0;
                    break;
                case "3x3":
                    BorderWidth = 500.0;
                    BorderHeight = 591.0;

                    ImageWidth = 454.0;
                    ImageHeight = 390.0;

                    WrapPanelWidth = 1950.0;

                    ScrollViewerHeight = 1860.0;
                    break;
                case "3x2":
                    BorderWidth = 658.0;
                    BorderHeight = 930.0;

                    ImageWidth = 600.0;
                    ImageHeight = 600.0;

                    WrapPanelWidth = 2200.0;

                    ScrollViewerHeight = 1980.0;
                    break;
                case "6x2":
                    BorderWidth = 539.0;
                    BorderHeight = 740.0;

                    ImageWidth = 539.0;
                    ImageHeight = 539.0;

                    WrapPanelWidth = 3526.0;

                    ScrollViewerHeight = 1540;
                    break;
                case "7x3":
                    BorderWidth = 454.0;
                    BorderHeight = 580.0;

                    ImageWidth = 454.0;
                    ImageHeight = 390.0;

                    WrapPanelWidth = 3526.0;

                    ScrollViewerHeight = 1860.0;
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
    }
}