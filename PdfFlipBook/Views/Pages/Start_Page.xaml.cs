using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using MoonPdfLib.Helper;
using MoonPdfLib.MuPdf;
using PdfFlipBook.Annotations;
using PdfFlipBook.Helper;
using PdfFlipBook.Helper.Singleton;
using PdfFlipBook.Models;
using PdfFlipBook.Utilities;

namespace PdfFlipBook.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Start_Page.xaml
    /// </summary>
    public partial class Start_Page : Page, INotifyPropertyChanged
    {

        public static readonly DependencyProperty ActualBooksProperty = DependencyProperty.Register(
            "ActualBooks", typeof(ObservableCollection<BookPDF>), typeof(Start_Page), new PropertyMetadata(new ObservableCollection<BookPDF>()));

        public ObservableCollection<BookPDF> ActualBooks
        {
            get { return (ObservableCollection<BookPDF>)GetValue(ActualBooksProperty); }
            set { SetValue(ActualBooksProperty, value); }
        }

        public static readonly DependencyProperty AllFoldersProperty = DependencyProperty.Register(
            "AllFolders", typeof(ObservableCollection<BookFolder>), typeof(Start_Page), new PropertyMetadata(default(ObservableCollection<BookFolder>)));

        public ObservableCollection<BookFolder> AllFolders
        {
            get { return (ObservableCollection<BookFolder>)GetValue(AllFoldersProperty); }
            set { SetValue(AllFoldersProperty, value); }
        }

        public static readonly DependencyProperty PageCountProperty = DependencyProperty.Register(
            "PageCount", typeof(int), typeof(Start_Page), new PropertyMetadata(default(int)));

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

        public static readonly DependencyProperty ActualRazdelProperty = DependencyProperty.Register(
            "ActualRazdel", typeof(string), typeof(Start_Page), new PropertyMetadata(default(string)));

        public string ActualRazdel
        {
            get { return (string)GetValue(ActualRazdelProperty); }
            set { SetValue(ActualRazdelProperty, value); }
        }


        public static Bitmap ExtractPage(IPdfSource source, int pageNumber, float zoomFactor = 1.0f, string password = null)
        {
            var pageNumberIndex = Math.Max(0, pageNumber - 1); // pages start at index 0

            using (var stream = new PdfFileStream(source))
            {
                ValidatePassword(stream.Document, password);

                IntPtr p = NativeMethods.LoadPage(stream.Document, pageNumberIndex); // loads the page
                var bmp = RenderPage(stream.Context, stream.Document, p, zoomFactor);
                NativeMethods.FreePage(stream.Document, p); // releases the resources consumed by the page

                return bmp;
            }
        }

        static Bitmap RenderPage(IntPtr context, IntPtr document, IntPtr page, float zoomFactor)
        {
            Rectangle pageBound = NativeMethods.BoundPage(document, page);
            Matrix ctm = new Matrix();
            IntPtr pix = IntPtr.Zero;
            IntPtr dev = IntPtr.Zero;

            var currentDpi = DpiHelper.GetCurrentDpi();
            var zoomX = zoomFactor * (currentDpi.HorizontalDpi / DpiHelper.DEFAULT_DPI);
            var zoomY = zoomFactor * (currentDpi.VerticalDpi / DpiHelper.DEFAULT_DPI);

            // gets the size of the page and multiplies it with zoom factors
            int width = (int)(pageBound.Width * zoomX);
            int height = (int)(pageBound.Height * zoomY);

            // sets the matrix as a scaling matrix (zoomX,0,0,zoomY,0,0)
            ctm.A = zoomX;
            ctm.D = zoomY;

            // creates a pixmap the same size as the width and height of the page
            pix = NativeMethods.NewPixmap(context, NativeMethods.FindDeviceColorSpace(context, "DeviceRGB"), width, height);
            // sets white color as the background color of the pixmap
            NativeMethods.ClearPixmap(context, pix, 0xFF);

            // creates a drawing device
            dev = NativeMethods.NewDrawDevice(context, pix);
            // draws the page on the device created from the pixmap
            NativeMethods.RunPage(document, page, dev, ctm, IntPtr.Zero);

            NativeMethods.FreeDevice(dev); // frees the resources consumed by the device
            dev = IntPtr.Zero;

            // creates a colorful bitmap of the same size of the pixmap
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var imageData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            unsafe
            { // converts the pixmap data to Bitmap data
                byte* ptrSrc = (byte*)NativeMethods.GetSamples(context, pix); // gets the rendered data from the pixmap
                byte* ptrDest = (byte*)imageData.Scan0;
                for (int y = 0; y < height; y++)
                {
                    byte* pl = ptrDest;
                    byte* sl = ptrSrc;
                    for (int x = 0; x < width; x++)
                    {
                        //Swap these here instead of in MuPDF because most pdf images will be rgb or cmyk.
                        //Since we are going through the pixels one by one anyway swap here to save a conversion from rgb to bgr.
                        pl[2] = sl[0]; //b-r
                        pl[1] = sl[1]; //g-g
                        pl[0] = sl[2]; //r-b
                                       //sl[3] is the alpha channel, we will skip it here
                        pl += 3;
                        sl += 4;
                    }
                    ptrDest += imageData.Stride;
                    ptrSrc += width * 4;
                }
            }
            bmp.UnlockBits(imageData);

            NativeMethods.DropPixmap(context, pix);
            bmp.SetResolution(currentDpi.HorizontalDpi, currentDpi.VerticalDpi);

            return bmp;
        }
        internal struct Matrix
        {
            public float A, B, C, D, E, F;
        }
        public static bool NeedsPassword(IPdfSource source)
        {
            using (var stream = new PdfFileStream(source))
            {
                return NeedsPassword(stream.Document);
            }
        }

        private static void ValidatePassword(IntPtr doc, string password)
        {
            if (NeedsPassword(doc) && NativeMethods.AuthenticatePassword(doc, password) == 0)
                throw new MissingOrInvalidPdfPasswordException();
        }

        private static bool NeedsPassword(IntPtr doc)
        {
            return NativeMethods.NeedsPassword(doc) != 0;
        }

        private sealed class PdfFileStream : IDisposable
        {
            const uint FZ_STORE_DEFAULT = 256 << 20;

            public IntPtr Context { get; private set; }
            public IntPtr Stream { get; private set; }
            public IntPtr Document { get; private set; }

            public PdfFileStream(IPdfSource source)
            {
                if (source is FileSource)
                {
                    var fs = (FileSource)source;
                    Context = NativeMethods.NewContext(IntPtr.Zero, IntPtr.Zero, FZ_STORE_DEFAULT); // Creates the context
                    Stream = NativeMethods.OpenFile(Context, fs.Filename); // opens file as a stream
                    Document = NativeMethods.OpenDocumentStream(Context, ".pdf", Stream); // opens the document
                }
                else if (source is MemorySource)
                {
                    var ms = (MemorySource)source;
                    Context = NativeMethods.NewContext(IntPtr.Zero, IntPtr.Zero, FZ_STORE_DEFAULT); // Creates the context
                    GCHandle pinnedArray = GCHandle.Alloc(ms.Bytes, GCHandleType.Pinned);
                    IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                    Stream = NativeMethods.OpenStream(Context, pointer, ms.Bytes.Length); // opens file as a stream
                    Document = NativeMethods.OpenDocumentStream(Context, ".pdf", Stream); // opens the document
                    pinnedArray.Free();
                }
            }

            public void Dispose()
            {
                NativeMethods.CloseDocument(Document); // releases the resources
                NativeMethods.CloseStream(Stream);
                NativeMethods.FreeContext(Context);
            }
        }
        private static class NativeMethods
        {
            const string DLL = "libmupdf.dll";

            [DllImport(DLL, EntryPoint = "fz_new_context", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr NewContext(IntPtr alloc, IntPtr locks, uint max_store);

            [DllImport(DLL, EntryPoint = "fz_free_context", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr FreeContext(IntPtr ctx);

            [DllImport(DLL, EntryPoint = "fz_open_file_w", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr OpenFile(IntPtr ctx, string fileName);

            [DllImport(DLL, EntryPoint = "fz_open_document_with_stream", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr OpenDocumentStream(IntPtr ctx, string magic, IntPtr stm);

            [DllImport(DLL, EntryPoint = "fz_close", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CloseStream(IntPtr stm);

            [DllImport(DLL, EntryPoint = "fz_close_document", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr CloseDocument(IntPtr doc);

            [DllImport(DLL, EntryPoint = "fz_count_pages", CallingConvention = CallingConvention.Cdecl)]
            public static extern int CountPages(IntPtr doc);

            [DllImport(DLL, EntryPoint = "fz_bound_page", CallingConvention = CallingConvention.Cdecl)]
            public static extern Rectangle BoundPage(IntPtr doc, IntPtr page);

            [DllImport(DLL, EntryPoint = "fz_clear_pixmap_with_value", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ClearPixmap(IntPtr ctx, IntPtr pix, int byteValue);

            [DllImport(DLL, EntryPoint = "fz_find_device_colorspace", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr FindDeviceColorSpace(IntPtr ctx, string colorspace);

            [DllImport(DLL, EntryPoint = "fz_free_device", CallingConvention = CallingConvention.Cdecl)]
            public static extern void FreeDevice(IntPtr dev);

            [DllImport(DLL, EntryPoint = "fz_free_page", CallingConvention = CallingConvention.Cdecl)]
            public static extern void FreePage(IntPtr doc, IntPtr page);

            [DllImport(DLL, EntryPoint = "fz_load_page", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr LoadPage(IntPtr doc, int pageNumber);

            [DllImport(DLL, EntryPoint = "fz_new_draw_device", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr NewDrawDevice(IntPtr ctx, IntPtr pix);

            [DllImport(DLL, EntryPoint = "fz_new_pixmap", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr NewPixmap(IntPtr ctx, IntPtr colorspace, int width, int height);

            [DllImport(DLL, EntryPoint = "fz_run_page", CallingConvention = CallingConvention.Cdecl)]
            public static extern void RunPage(IntPtr doc, IntPtr page, IntPtr dev, Matrix transform, IntPtr cookie);

            [DllImport(DLL, EntryPoint = "fz_drop_pixmap", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DropPixmap(IntPtr ctx, IntPtr pix);

            [DllImport(DLL, EntryPoint = "fz_pixmap_samples", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetSamples(IntPtr ctx, IntPtr pix);

            [DllImport(DLL, EntryPoint = "fz_needs_password", CallingConvention = CallingConvention.Cdecl)]
            public static extern int NeedsPassword(IntPtr doc);

            [DllImport(DLL, EntryPoint = "fz_authenticate_password", CallingConvention = CallingConvention.Cdecl)]
            public static extern int AuthenticatePassword(IntPtr doc, string password);

            [DllImport(DLL, EntryPoint = "fz_open_memory", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr OpenStream(IntPtr ctx, IntPtr data, int len);
        }
        internal struct Rectangle
        {
            public float Left, Top, Right, Bottom;

            public float Width { get { return this.Right - this.Left; } }
            public float Height { get { return this.Bottom - this.Top; } }

        }

        public Start_Page()
        {
            InitializeComponent();
            AuthorBookRB.IsChecked = true;

            

            var a = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\PDFs").ToList();
            AllFolders = new ObservableCollection<BookFolder>();
            foreach (string s in a)
            {
                BookFolder BF = new BookFolder()
                {
                    Title = s.Split('\\').Last(),
                    Icon = new DisposableImage(Directory.GetFiles(s + "\\Logo\\")[0])
                };
                AllFolders.Add(BF);
            }

        }

        static async void UpdaePhotosAsync()
        {

            //await Task.Run(() => UpdatePhotos()); // выполняется асинхронно
        }


        public void UpdatePhotos()
        {
            if (App.CurrentApp.AllBooks.Count == 0)
            {
                int pageCount;
                ObservableCollection<BookPDF> AllBooks2 = new ObservableCollection<BookPDF>();
                var pdfFolders = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\PDFs\\");
                foreach (var pdfFolder in pdfFolders)
                {
                    var pdfFiles = Directory.GetFiles(pdfFolder);
                    foreach (var pdfFile in pdfFiles)
                    {


                        IPdfSource pdfDoc = new FileSource(pdfFile);
                        using (var stream = new PdfFileStream(pdfDoc))
                        {
                            ValidatePassword(stream.Document, null);

                            pageCount = NativeMethods.CountPages(stream
                                .Document); // gets the number of pages in the document
                        }

                        string folderToImages = Directory.GetCurrentDirectory() + "\\Temp\\" +
                                                pdfFile.Split('\\').Last().Replace(".pdf", "");

                        if (!Directory.Exists(folderToImages))
                            Directory.CreateDirectory(folderToImages);
                        if (Directory.GetFiles(folderToImages).ToList().Count != pageCount)
                        {
                            for (int i = 1; i < pageCount + 1; i++)
                            {
                                var a = ExtractPage(pdfDoc, i, 4, null);
                                a.Save(folderToImages + "\\" + i + ".jpg", ImageFormat.Jpeg);
                                System.GC.Collect();
                                System.GC.WaitForPendingFinalizers();
                            }
                        }

                        DisposableImage icon = new DisposableImage(Directory.GetFiles(folderToImages)[0]);
                        string title = pdfFile.Split('\\').Last().Replace(".pdf", "");
                        BookPDF bpdf = new BookPDF()
                        { Title = title, Icon = icon, FullPath = pdfFolder, Text = ReadPdfFile(pdfFile), Author = title.Split('.').First(), Book = title.Split('.').Last() };
                        AllBooks2.Add(bpdf);

                    }


                }
                AllBooks = AllBooks2;
                App.CurrentApp.AllBooks = AllBooks2;
            }
            else
            {
                AllBooks = App.CurrentApp.AllBooks;
            }
            App.CurrentApp.IsLoading = false;
            return;
        }

        public string ReadPdfFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }

        private ICommand _bookCommand;
        public ICommand BookCommand =>
            _bookCommand ??= (_bookCommand = new Command(c =>
            {
                GlobalSettings.Instance.Books = new ObservableCollection<BookPDF>(App.CurrentApp.ActualBooks = AllBooks.Where(x => x.FullPath.Contains(c.ToString())).ToList());
                var BookData = Tuple.Create(c.ToString(), SettingsModel);
                ExecuteNavigation(BookData);

                foreach (var actualBook in ActualBooks)
                {
                    actualBook.Icon.Dispose();
                }

                NameTB.Text = "";
                SearchResultGrid.Visibility = Visibility.Collapsed;
                FoldersItemsControl.Visibility = Visibility.Visible;
                ActualBooks.Clear();
                ActualBooks = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }));

        private ICommand _hideKeyboardCommand;

        public ICommand HideKeyboardCommand =>
            _hideKeyboardCommand ??= (_hideKeyboardCommand = new Command(c =>
            {
                HideButton.Visibility = Visibility.Collapsed;
                FoldersItemsControl.Focus();
                var sb = new Storyboard();
                var animation = new ThicknessAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.75)),
                    To = new Thickness(0, 0, 0, -800),
                    From = new Thickness(0, 0, 0, 200),
                    DecelerationRatio = 0.9f,

                };
                Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
                sb.Children.Add(animation);
                sb.Begin(CoolKeyBoard);
            }));

        private ICommand _razdelCommand;
        public ICommand RazdelCommand =>
            _razdelCommand ?? (_razdelCommand = new Command(c =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                ActualRazdel = c.ToString();
                var actualBooks = App.CurrentApp.ActualBooks = AllBooks.Where(x => x.FullPath.Contains(c.ToString())).ToList();
                GlobalSettings.Instance.Books = new ObservableCollection<BookPDF>(actualBooks);
                var razdelData = Tuple.Create(ActualRazdel, actualBooks, SettingsModel);
                ExecuteNavigation(razdelData);
            }));

        private void ExecuteNavigation(object data)
        {
            CommonCommands.NavigateCommand.Execute(data);
        }

        ////private ICommand _backCommand;

        ////public ICommand WriteJsonFileAndBackCommand =>
        ////    _backCommand ?? (_backCommand = new Command(c =>
        ////    {
        ////        ActualRazdel = "";
        ////        SearchOptionSP.Visibility = Visibility.Visible;
        ////        LoupeImage.Visibility = Visibility.Visible;
        ////        NameTB.Visibility = Visibility.Visible;
        ////        BooksItemsControl.Visibility = Visibility.Collapsed;
        ////        BackButton.Visibility = Visibility.Collapsed;
        ////        FoldersItemsControl.Visibility = Visibility.Visible;
        ////        //foreach (var actualBook in ActualBooks)
        ////        //{
        ////        //  actualBook.Icon.Dispose();
        ////        //}
        ////        GC.Collect();

        ////    }));

        private ICommand _stopTimerCommand;
        private ICommand _startTimerCommand;

        public ICommand StopTimerCommand => _stopTimerCommand ??= (_stopTimerCommand = new Command(a =>
        {
            _timer.Tick -= Timer;
            _timer.Stop();
            _sec = 0;
        }));



        public ICommand StartTimerCommand => _startTimerCommand ??= (_startTimerCommand = new Command(a =>
        {
            _timer?.Stop();
            _sec = 0;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer;
            _timer.Start();
        }));

        DispatcherTimer _timer = new DispatcherTimer();
        private int _sec = 0;

        private void Timer(object sender, EventArgs eventArgs)
        {
            _sec++;
            if (_sec >= 7)
                Application.Current.Shutdown();
        }

        private ICommand _stopTimer2Command;
        private ICommand _startTimer2Command;

        public ICommand StopTimer2Command => _stopTimer2Command ??= (_stopTimer2Command = new Command(a =>
        {
            _timer2.Tick -= Timer2;
            _timer2.Stop();
            _sec2 = 0;
        }));

        public ICommand StartTimer2Command => _startTimer2Command ??= (_startTimer2Command = new Command(a =>
        {
            _timer2?.Stop();
            _sec2 = 0;
            _timer2.Interval = TimeSpan.FromSeconds(1);
            _timer2.Tick += Timer2;
            _timer2.Start();
        }));

        DispatcherTimer _timer2 = new DispatcherTimer();
        private int _sec2 = 0;

        private void Timer2(object sender, EventArgs eventArgs)
        {
            _sec2++;
            if (_sec2 < 4) return;
            CommonCommands.NavigateCommand.Execute(SettingsModel);
            _timer2.Stop();

        }


        private ObservableCollection<BookPDF> _allBooks;
        public ObservableCollection<BookPDF> AllBooks
        {
            get => _allBooks ??= (_allBooks = new ObservableCollection<BookPDF>());
            set
            {
                _allBooks = value;
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


        private async void Start_Page_OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Run((() =>
            {
                App.CurrentApp.IsLoading = true;
                UpdatePhotos();
            }));
            App.CurrentApp.IsLoading = false;

            var helper = new JsonHelper();
            var jsonPath = "Settings/Settings.json";
            var directoryPath = "Settings";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (!File.Exists(jsonPath))
            {
                var settings = new SettingsModel
                {
                    InactivityTime = "60",
                    IntervalSwitchPage = "5",
                    Password = "1234",
                    Repeat = false,
                    NextPage = true
                };
                helper.WriteJsonToFile(jsonPath, settings, false);

                SettingsModel = settings;
            }
            else
            {
                var settings = helper.ReadJsonFromFile<SettingsModel>(jsonPath);
                SettingsModel = settings;
            }

            //List<BookPDF> AllBooks2 = new List<BookPDF>();
            //var pdfFolders = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\PDFs");
            //foreach (var pdfFolder in pdfFolders)
            //{
            //    string folderToImages = Directory.GetCurrentDirectory() + "\\Temp\\" +
            //                            pdfFolder.Split('\\').Last().Replace(".pdf", "");
            //    if (Directory.Exists(folderToImages))
            //    {
            //        DisposableImage icon = new DisposableImage(Directory.GetFiles(folderToImages)[0]);
            //        string title = pdfFolder.Split('\\').Last().Replace(".pdf", "");
            //        BookPDF bpdf = new BookPDF() {Title = title, Icon = icon};
            //        AllBooks2.Add(bpdf);
            //    }

            //}
            //AllBooks = AllBooks2;
            //App.CurrentApp.IsLoading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NameTB_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameTB.Text.Length == 0)
            {
                SearchResultGrid.Visibility = Visibility.Collapsed;
                FoldersItemsControl.Visibility = Visibility.Visible;

            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand =>
            _searchCommand ??= new Command(async c =>
            {
                SearchResultGrid.Visibility = Visibility.Visible;
                FoldersItemsControl.Visibility = Visibility.Collapsed;

                string searchQuery = NameTB.Text.ToLower();

                if (AuthorBookRB.IsChecked == true)
                {
                    var filteredBooks = await Task.Run(() =>
                        AllBooks
                            .Where(x => x.Author.ToLower().Contains(searchQuery) ||
                                        x.Title.ToLower().Contains(searchQuery))
                            .ToList());

                    ActualBooks.Clear();
                    foreach (var book in filteredBooks)
                    {
                        ActualBooks.Add(book);
                    }
                }
                else if (TextBookRB.IsChecked == true)
                {
                    var filteredBooks = await Task.Run(() =>
                        AllBooks
                            .Where(x => x.Text.ToLower().Contains(searchQuery))
                            .ToList());

                    ActualBooks.Clear();
                    foreach (var book in filteredBooks)
                    {
                        ActualBooks.Add(book);
                    }
                }
            });

        private void NameTB_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (CoolKeyBoard.Margin.Bottom < -200)
            {
                var sb = new Storyboard();
                var animation = new ThicknessAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.75)),
                    From = new Thickness(0, 0, 0, -800),
                    To = new Thickness(0, 0, 0, 200),
                    DecelerationRatio = 0.9f,

                };
                Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
                sb.Children.Add(animation);
                sb.Begin(CoolKeyBoard);
                HideButton.Visibility = Visibility.Visible;
            }
        }


    }
}
