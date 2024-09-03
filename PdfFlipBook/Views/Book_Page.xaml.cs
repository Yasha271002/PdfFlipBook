using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using PdfFlipBook.Utilities;
using Path = System.IO.Path;

namespace PdfFlipBook.Views
{
    /// <summary>
    /// Логика взаимодействия для Book_Page.xaml
    /// </summary>
    public partial class Book_Page : Page
    {
        public Book_Page(string bookTitle)
        {
            InitializeComponent();
            BookTitle = bookTitle;
            AllPages = new ObservableCollection<DisposableImage>();
           
            AllPhotos = new List<string>(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" + bookTitle ).ToList().OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x))));
            foreach (var s in AllPhotos)
            {
                AllPages.Add(new DisposableImage(s));
            }
            if(AllPages.Count>30)
            {
                for (int i = 30; i < AllPages.Count; i++)
                {
                    AllPages[i].Dispose();
                }
            }
            GC.Collect();
            App.CurrentApp.IsLoading = false;


        }

        public static readonly DependencyProperty BookTitleProperty = DependencyProperty.Register(
            "BookTitle", typeof(string), typeof(Book_Page), new PropertyMetadata(default(string)));

        public string BookTitle
        {
            get { return (string) GetValue(BookTitleProperty); }
            set { SetValue(BookTitleProperty, value); }
        }

        private ICommand _backCommand;

        public ICommand BackCommand =>
            _backCommand ?? (_backCommand = new Command(c =>
            {
                NavigationService?.GoBack();
                if (AllPages != null)
                {
                    foreach (var allBooks in AllPages)
                    {
                        allBooks.Dispose();

                    }
                }
                GC.Collect();
            }));

        private ICommand _insertPageCommand;

        public ICommand InsertPageCommand =>
            _insertPageCommand ?? (_insertPageCommand = new Command(c =>
            {
                InsertGrid.Visibility = Visibility.Visible;
            }));

        private ICommand _closeInsertCommand;

        public ICommand CloseInsertCommand =>
            _closeInsertCommand ?? (_closeInsertCommand = new Command(c =>
            {
                InsertGrid.Visibility = Visibility.Collapsed;
                KK.Text = string.Empty;
                onScreenKeyboard.Text = string.Empty;
            }));

        private ICommand _deleteCommand;

        public ICommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new Command(c =>
            {
                if (KK.Text != string.Empty)
                {
                    string qweq = KK.Text.Remove(KK.Text.Length - 1);
                    KK.Text = qweq;
                    onScreenKeyboard.Text = qweq;

                }
            }));


        private ICommand _toPageCommand;

        public ICommand ToPageCommand =>
            _toPageCommand ?? (_toPageCommand = new Command(c =>
            {
                int page = int.Parse(KK.Text);
                if ( page> AllPages.Count)
                {
                    var animation = new DoubleAnimation
                    {
                        From=0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(1.3),
                        AutoReverse = true
                    };
                    TB.BeginAnimation(UIElement.OpacityProperty, animation);
                }
                else
                {
                    AllPages = new ObservableCollection<DisposableImage>();

                    AllPhotos = new List<string>(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" + BookTitle).ToList().OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x))));
                    foreach (var s in AllPhotos)
                    {
                        AllPages.Add(new DisposableImage(s));
                    }
                    if (AllPages.Count > 30)
                    {
                        for (int i = 30; i < AllPages.Count; i++)
                        {
                            AllPages[i].Dispose();
                        }
                    }
                    GC.Collect();
                    var ostatok = page % 2;
                    if (ostatok == 1)
                        page--;


                    Book.CurrentSheetIndex = page/2;
                    int index = Book.CurrentSheetIndex;
                    try
                    {

                        AllPages[index * 2 - 4].Dispose();
                        AllPages[index * 2 - 5].Dispose();
                        GC.Collect();
                        AllPages.RemoveAt(index * 2 - 3);
                        AllPages.Insert(index * 2 - 3, new DisposableImage(AllPhotos[index * 2 - 3]));
                        AllPages.RemoveAt(index * 2 - 2);
                        AllPages.Insert(index * 2 - 2, new DisposableImage(AllPhotos[index * 2 - 2]));
                        AllPages.RemoveAt(index * 2 - 1);
                        AllPages.Insert(index * 2 - 1, new DisposableImage(AllPhotos[index * 2 - 1]));
                        AllPages.RemoveAt(index * 2);
                        AllPages.Insert(index * 2, new DisposableImage(AllPhotos[index * 2]));
                        AllPages.RemoveAt(index * 2 + 1);
                        AllPages.Insert(index * 2 + 1, new DisposableImage(AllPhotos[index * 2 + 1]));
                        AllPages.RemoveAt(index * 2 + 2);
                        AllPages.Insert(index * 2 + 2, new DisposableImage(AllPhotos[index * 2 + 2]));
                        AllPages.RemoveAt(index * 2 + 3);
                        AllPages.Insert(index * 2 + 3, new DisposableImage(AllPhotos[index * 2 + 3]));
                        AllPages.RemoveAt(index * 2 + 4);
                        AllPages.Insert(index * 2 + 4, new DisposableImage(AllPhotos[index * 2 + 4]));

                    }
                    catch (Exception exception)
                    {

                    }
                    CloseInsertCommand.Execute(null);
                }
                
            }));

        public static readonly DependencyProperty AllPagesProperty = DependencyProperty.Register(
            "AllPages", typeof(ObservableCollection< DisposableImage>), typeof(Book_Page), new PropertyMetadata(default(ObservableCollection< DisposableImage>)));

        public ObservableCollection< DisposableImage> AllPages
        {
            get { return (ObservableCollection< DisposableImage>) GetValue(AllPagesProperty); }
            set { SetValue(AllPagesProperty, value); }
        }

        private void Book_Page_OnLoaded(object sender, RoutedEventArgs e)
        {
           // App.CurrentApp.IsLoading = true;
        }

        public static readonly DependencyProperty AllPhotosProperty = DependencyProperty.Register(
            "AllPhotos", typeof(List<string>), typeof(Book_Page), new PropertyMetadata(default(List<string>)));

        public List<string> AllPhotos
        {
            get { return (List<string>) GetValue(AllPhotosProperty); }
            set { SetValue(AllPhotosProperty, value); }
        }

        private void Book_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int index = Book.CurrentSheetIndex;
           // MessageBox.Show(index.ToString());
           var xPosition = e.GetPosition(this).X;
           if (AllPages.Count > 30)
            {
                if (xPosition < 1920)
               {
                   if (StartPointX > 1920)
                   {
                       try
                       {
                           
                               AllPages[index * 2 - 4].Dispose();
                               AllPages[index * 2 - 5].Dispose();
                               GC.Collect();
                               AllPages.RemoveAt(index * 2 + 1);
                               AllPages.Insert(index * 2 + 1, new DisposableImage(AllPhotos[index * 2 + 1]));
                               AllPages.RemoveAt(index * 2 + 2);
                               AllPages.Insert(index * 2 + 2, new DisposableImage(AllPhotos[index * 2 + 2]));
                               AllPages.RemoveAt(index * 2 + 3);
                               AllPages.Insert(index * 2 + 3, new DisposableImage(AllPhotos[index * 2 + 3]));
                               AllPages.RemoveAt(index * 2 + 4);
                               AllPages.Insert(index * 2 + 4, new DisposableImage(AllPhotos[index * 2 + 4]));
                           
                       }
                       catch (Exception exception)
                       {

                       }


                   }
               }

               else
               {
                   if (StartPointX < 1920)
                   {
                       try
                       {
                          
                               AllPages[index * 2 + 3].Dispose();
                               AllPages[index * 2 + 4].Dispose();
                               GC.Collect();

                               AllPages.RemoveAt(index * 2 - 4);
                               AllPages.Insert(index * 2 - 4, new DisposableImage(AllPhotos[index * 2 - 4]));
                               AllPages.RemoveAt(index * 2 - 5);
                               AllPages.Insert(index * 2 - 5, new DisposableImage(AllPhotos[index * 2 - 5]));
                               AllPages.RemoveAt(index * 2 - 6);
                               AllPages.Insert(index * 2 - 6, new DisposableImage(AllPhotos[index * 2 - 6]));
                               AllPages.RemoveAt(index * 2 - 7);
                               AllPages.Insert(index * 2 - 7, new DisposableImage(AllPhotos[index * 2 - 7]));

                        }
                       catch (Exception exception)
                       {

                       }



                   }

               }
           }


            

        }

        public static readonly DependencyProperty StartPointXProperty = DependencyProperty.Register(
            "StartPointX", typeof(double), typeof(Book_Page), new PropertyMetadata(default(double)));

        public double StartPointX
        {
            get { return (double) GetValue(StartPointXProperty); }
            set { SetValue(StartPointXProperty, value); }
        }




        public static readonly DependencyProperty BookIndexProperty = DependencyProperty.Register(
            "BookIndex", typeof(int), typeof(Book_Page), new PropertyMetadata(default(int)));

        public int BookIndex
        {
            get { return (int) GetValue(BookIndexProperty); }
            set { SetValue(BookIndexProperty, value); }
        }

        private void Book_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPointX = e.GetPosition(this).X;
        }
    }
}
