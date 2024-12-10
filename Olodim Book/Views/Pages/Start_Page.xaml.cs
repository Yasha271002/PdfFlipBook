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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using MoonPdfLib.MuPdf;
using Newtonsoft.Json;
using PdfFlipBook.Helper;
using PdfFlipBook.Helper.Singleton;
using PdfFlipBook.Models;
using PdfFlipBook.Properties;
using PdfFlipBook.Utilities;
using Color = System.Windows.Media.Color;
using Path = System.IO.Path;

namespace PdfFlipBook.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Start_Page.xaml
    /// </summary>
    public partial class Start_Page : Page, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ActualBooksProperty = DependencyProperty.Register(
            "ActualBooks", typeof(ObservableCollection<BookPDF>), typeof(Start_Page),
            new PropertyMetadata(new ObservableCollection<BookPDF>()));

        public ObservableCollection<BookPDF> ActualBooks
        {
            get { return (ObservableCollection<BookPDF>)GetValue(ActualBooksProperty); }
            set { SetValue(ActualBooksProperty, value); }
        }

        public static readonly DependencyProperty AllFoldersProperty = DependencyProperty.Register(
            "AllFolders", typeof(ObservableCollection<BookFolder>), typeof(Start_Page),
            new PropertyMetadata(default(ObservableCollection<BookFolder>)));

        public ObservableCollection<BookFolder> AllFolders
        {
            get { return (ObservableCollection<BookFolder>)GetValue(AllFoldersProperty); }
            set
            {
                SetValue(AllFoldersProperty, value);
                OnPropertyChanged();
            }
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


        public static Bitmap ExtractPage(IPdfSource source, int pageNumber, float zoomFactor = 1.0f,
            string password = null)
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
            pix = NativeMethods.NewPixmap(context, NativeMethods.FindDeviceColorSpace(context, "DeviceRGB"), width,
                height);
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
            var imageData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            unsafe
            {
                // converts the pixmap data to Bitmap data
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
                    Context = NativeMethods.NewContext(IntPtr.Zero, IntPtr.Zero,
                        FZ_STORE_DEFAULT); // Creates the context
                    Stream = NativeMethods.OpenFile(Context, fs.Filename); // opens file as a stream
                    Document = NativeMethods.OpenDocumentStream(Context, ".pdf", Stream); // opens the document
                }
                else if (source is MemorySource)
                {
                    var ms = (MemorySource)source;
                    Context = NativeMethods.NewContext(IntPtr.Zero, IntPtr.Zero,
                        FZ_STORE_DEFAULT); // Creates the context
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

            [DllImport(DLL, EntryPoint = "fz_open_file_w", CharSet = CharSet.Unicode,
                CallingConvention = CallingConvention.Cdecl)]
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

            [DllImport(DLL, EntryPoint = "fz_open_memory", CharSet = CharSet.Unicode,
                CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr OpenStream(IntPtr ctx, IntPtr data, int len);
        }

        internal struct Rectangle
        {
            public float Left, Top, Right, Bottom;

            public float Width
            {
                get { return this.Right - this.Left; }
            }

            public float Height
            {
                get { return this.Bottom - this.Top; }
            }
        }

        public Start_Page()
        {
            InitializeComponent();
            AuthorBookRB.IsChecked = true;

            CloseSearchButton.Visibility = Visibility.Collapsed;

            if (Directory.Exists("PDFs"))
                Directory.CreateDirectory("PDFs");

            var a = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\PDFs").ToList();
            AllFolders = new ObservableCollection<BookFolder>();
            
            CreateFolders(a);

            foreach (var BF in a.Select(s => new BookFolder()
                     {
                         Title = s.Split('\\').Last(),
                         Icon = Directory.GetFiles(s + "\\Logo\\")[0].ToString(),
                         Background = Directory.GetFiles(s + "\\Background\\").FirstOrDefault()!.ToString(),
                         Sound = Directory.GetFiles(s + "\\Sound\\").FirstOrDefault()!.ToString()
            }))
            {
                AllFolders.Add(BF);
            }

            LoadFoldersOrder();
        }

        private void CreateFolders(List<string> pathList)
        {
            foreach (var directory in pathList)
            {
                var backgroundPath = Path.Combine(directory, "Background");
                var logoPath = Path.Combine(directory, "Logo");
                var soundPath = Path.Combine(directory, "Sound");

                if (!Directory.Exists(backgroundPath))
                    Directory.CreateDirectory(backgroundPath);

                if (!Directory.Exists(logoPath))
                    Directory.CreateDirectory(logoPath);

                if (!Directory.Exists(soundPath))
                    Directory.CreateDirectory(soundPath);
            }
        }

        private async Task UpdatePhotos()
        {
            if (App.CurrentApp.AllBooks.Count == 0)
            {
                var allBooks = new ObservableCollection<BookPDF>();
                var pdfFolders = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\PDFs\\");

                await Task.Run(() =>
                {
                    Task.WaitAll((from pdfFolder in pdfFolders
                        let pdfFiles = Directory.GetFiles(pdfFolder)
                        from pdfFile in pdfFiles
                        select Task.Run(() =>
                        {
                            var bookPdf = ProcessPdfFile(pdfFile, pdfFolder);

                            if (bookPdf == null) return;
                            lock (allBooks)
                            {
                                allBooks.Add(bookPdf);
                            }
                        })).ToArray());
                });

                AllBooks = allBooks;
                App.CurrentApp.AllBooks = allBooks;
            }
            else
            {
                AllBooks = App.CurrentApp.AllBooks;
            }

            App.CurrentApp.IsLoading = false;
        }

        private BookPDF ProcessPdfFile(string pdfFile, string pdfFolder)
        {
            try
            {
                IPdfSource pdfDoc = new FileSource(pdfFile);
                int pageCount;

                using (var stream = new PdfFileStream(pdfDoc))
                {
                    ValidatePassword(stream.Document, null);
                    pageCount = NativeMethods.CountPages(stream.Document);
                }

                string folderToImages = Path.Combine(Directory.GetCurrentDirectory(), "Temp",
                    Path.GetFileNameWithoutExtension(pdfFile));
                if (!Directory.Exists(folderToImages))
                    Directory.CreateDirectory(folderToImages);

                var existingImages = Directory.GetFiles(folderToImages);
                if (existingImages.Length != pageCount)
                {
                    ExtractImagesFromPdf(pdfDoc, pageCount, folderToImages);
                }

                string iconPath = Directory.GetFiles(folderToImages).FirstOrDefault();
                if (iconPath == null) return null;

                string title = Path.GetFileNameWithoutExtension(pdfFile);
                string author = title.Split('.').First();
                string bookName = title.Split('.').Last();

                string textContent = ReadPdfFile(pdfFile);

                var bookPdf = new BookPDF
                {
                    Title = title,
                    Icon = iconPath,
                    FullPath = pdfFolder,
                    Text = textContent,
                    Author = author,
                    Book = bookName
                };

                return bookPdf;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {pdfFile}: {ex.Message}");
                return null;
            }
        }

        private void ExtractImagesFromPdf(IPdfSource pdfDoc, int pageCount, string folderToImages)
        {
            for (int i = 1; i <= pageCount; i++)
            {
                try
                {
                    var pageImage = ExtractPage(pdfDoc, i, 2, null);
                    string imagePath = Path.Combine(folderToImages, $"{i}.jpg");
                    pageImage.Save(imagePath, ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error extracting page {i}: {ex.Message}");
                }
            }
        }

        private string ReadPdfFile(string fileName)
        {
            var text = new StringBuilder();

            if (!File.Exists(fileName)) return text.ToString();

            var pdfReader = new PdfReader(fileName);

            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8,
                    Encoding.Default.GetBytes(currentText)));
                text.Append(currentText);
            }

            pdfReader.Close();
            return text.ToString();
        }

        private ICommand _bookCommand;

        public ICommand BookCommand =>
            _bookCommand ??= (_bookCommand = new Command(c =>
            {
                GlobalSettings.Instance.Books = new ObservableCollection<BookPDF>(App.CurrentApp.ActualBooks =
                    AllBooks.Where(x => x.FullPath.Contains(c.ToString())).ToList());

                var book = AllBooks.FirstOrDefault(name => name.Title == c.ToString());

                var folderName = book.FullPath;
                var word = folderName.Split( 's').LastOrDefault();
                var lastWord = word.Split( '\\').LastOrDefault();
                SelectRazdel = AllFolders.FirstOrDefault(name => name.Title == lastWord); 

                var BookData = Tuple.Create(c.ToString(), SettingsModel, SelectRazdel);
                ExecuteNavigation(BookData);

                NameTB.Text = "";
                SearchResultGrid.Visibility = Visibility.Collapsed;
                FoldersItemsControl.Visibility = Visibility.Visible;
                ActualBooks.Clear();
                ActualBooks = null;
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
                ActualRazdel = c.ToString();
                SelectRazdel = AllFolders.FirstOrDefault(f => f.Title == c.ToString());

                var actualBooks = new ObservableCollection<BookPDF>(App.CurrentApp.ActualBooks =
                    new List<BookPDF>(AllBooks.Where(x => x.FullPath.Contains(c.ToString()))));

                GlobalSettings.Instance.Books = new ObservableCollection<BookPDF>(actualBooks);
                var razdelData = Tuple.Create(ActualRazdel, actualBooks,SettingsModel, SelectRazdel);
                ExecuteNavigation(razdelData);
            }));

        private void ExecuteNavigation(object data)
        {
            CommonCommands.NavigateCommand.Execute(data);
        }

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

        private ObservableCollection<BookPDF> AllBooks
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

        private AudioHelper _audioHelper;

        private void CheckExist(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        private void Start_Page_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _audioHelper.Exit();
        }

        private async void Start_Page_OnLoaded(object sender, RoutedEventArgs e)
        {
            App.CurrentApp.IsLoading = true;
            await UpdatePhotos();

            var helper = new JsonHelper();

            var folderList = new List<string>
            {
                "Settings",
                "Sound/BackgroundSound",
                "Sound/SwitchPageSound",
                "Sound"
            };

            foreach (var folder in folderList)
            {
                CheckExist(folder);
            }

            var jsonPath = "Settings/Settings.json";
            var backgroundPath = "background.json";

            var backgroundSound = "Sound/BackgroundSound/MainBackground.mp3";
            var switchSound = "Sound/SwitchPageSound/SwitchPageSound.mp3";

            if (!File.Exists(jsonPath))
            {
                var settings = new SettingsModel
                {
                    InactivityTime = "60",
                    IntervalSwitchPage = "5",
                    Password = "1234",
                    Repeat = false,
                    NextPage = true,
                    Volume = 0.5f,
                    SelectedBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    SelectedColor = Color.FromRgb(255, 255, 255),
                    MainBackgroundSoundPath = Path.GetFullPath(backgroundSound),
                    SwitchSoundPath = Path.GetFullPath(switchSound),
                    Hue = 0,
                    Brightness = 100,
                    Saturation = 0,
                };
                helper.WriteJsonToFile(jsonPath, settings, false);

                SettingsModel = helper.ReadJsonFromFile<SettingsModel>(jsonPath);

               

                SettingsModel.JsonColor = SettingsModel.SelectedColor;
                SettingsModel.JsonBrush = SettingsModel.SelectedBrush;
                SettingsModel.JsonHue = SettingsModel.Hue;
                SettingsModel.JsonBrightness = SettingsModel.Brightness;
                SettingsModel.JsonHueBrush = SettingsModel.HueBrush;
                SettingsModel.JsonSaturation = SettingsModel.Saturation;

            }
            else
            {
                var settings = helper.ReadJsonFromFile<SettingsModel>(jsonPath);
                SettingsModel = settings;

                SettingsModel.JsonColor = SettingsModel.SelectedColor;
                SettingsModel.JsonBrush = SettingsModel.SelectedBrush;
                SettingsModel.JsonHue = SettingsModel.Hue;
                SettingsModel.JsonBrightness = SettingsModel.Brightness;
                SettingsModel.JsonHueBrush = SettingsModel.HueBrush;
                SettingsModel.JsonSaturation = SettingsModel.Saturation;
            }

            GlobalSettings.Instance.Settings = SettingsModel;


            _audioHelper = new AudioHelper(SettingsModel.MainBackgroundSoundPath, SettingsModel.Volume);
            PlaySound();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NameTB_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameTB.Text.Length != 0) return;

            HideButton.Visibility = Visibility.Visible;
            SearchResultGrid.Visibility = Visibility.Collapsed;
            FoldersItemsControl.Visibility = Visibility.Visible;
        }

        private ICommand _searchCommand;

        public ICommand SearchCommand =>
            _searchCommand ??= new Command(async c =>
            {
                ActualBooks = new();

                SearchResultGrid.Visibility = Visibility.Visible;
                FoldersItemsControl.Visibility = Visibility.Collapsed;

                var searchQuery = NameTB.Text.ToLower();

                if (AuthorBookRB.IsChecked == true)
                {
                    var filteredBooks = await Task.Run(() =>
                        AllBooks
                            .Where(x => x.Author.ToLower().Contains(searchQuery) ||
                                        x.Title.ToLower().Contains(searchQuery))
                            .ToList());

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

        private ICommand _hideSearchGridCommand;

        public ICommand HideSearchGridCommand => new Command(f =>
        {
            NameTB.Text = string.Empty;
            SearchResultGrid.Visibility = Visibility.Collapsed;
            CloseSearchButton.Visibility = Visibility.Collapsed;
            FoldersItemsControl.Visibility = Visibility.Visible;
        });

        private void NameTB_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(CoolKeyBoard.Margin.Bottom < -200)) return;

            CloseSearchButton.Visibility = Visibility.Visible;

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

        #region Drag

        private const string FoldersOrderFileName = "folders_order.json";

        private void SaveFoldersOrder()
        {
            try
            {
                var json = JsonConvert.SerializeObject(AllFolders.Select(f => f.Title).ToList());
                File.WriteAllText(FoldersOrderFileName, json);
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadFoldersOrder()
        {
            try
            {
                if (!File.Exists(FoldersOrderFileName)) return;
                var json = File.ReadAllText(FoldersOrderFileName);
                var savedOrder = JsonConvert.DeserializeObject<List<string>>(json);


                var orderedFolders = new ObservableCollection<BookFolder>();

                foreach (var folder in savedOrder.Select(title => AllFolders.FirstOrDefault(f => f.Title == title))
                             .Where(folder => folder != null))
                {
                    orderedFolders.Add(folder);
                }

                foreach (var folder in AllFolders)
                {
                    if (!orderedFolders.Contains(folder))
                    {
                        orderedFolders.Add(folder);
                    }
                }

                AllFolders = orderedFolders;
            }
            catch (Exception ex)
            {
            }
        }

        private bool _isDragging;
        public bool IsDragging
        {
            get => _isDragging;
            set
            {
                _isDragging = value;
                OnPropertyChanged();
            }
        }

        private ICommand _enableDraggingCommand;
        public ICommand EnableDraggingCommand => _enableDraggingCommand ??= new Command(f =>
        {
            IsDragging = !IsDragging;
        });


        private ICommand _cancelDraggingCommand;
        public ICommand CancelDraggingCommand => _cancelDraggingCommand ??= new Command(f =>
        {
            IsDragging = !IsDragging;
            LoadFoldersOrder();
        });

        private ICommand _saveDraggingCommand;
        public ICommand SaveDraggingCommand => _saveDraggingCommand ??= new Command(f =>
        {
            IsDragging = !IsDragging;
            SaveFoldersOrder();
        });

        #endregion


        private void FoldersItemsControl_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DragButton.Visibility = SearchResultGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }
}