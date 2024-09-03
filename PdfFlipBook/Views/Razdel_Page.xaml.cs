using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PdfFlipBook.Annotations;
using PdfFlipBook.Models;
using PdfFlipBook.Utilities;

namespace PdfFlipBook.Views
{
    /// <summary>
    /// Логика взаимодействия для Razdel_Page.xaml
    /// </summary>
    public partial class Razdel_Page : Page,INotifyPropertyChanged
    {
        public Razdel_Page(string razdel,List<BookPDF> actualBooks)
        {
            InitializeComponent();
            ActualBack = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Background")[0];

            ActualRazdel = razdel;
           
            GetBooks(actualBooks, razdel);
        }

        public static readonly DependencyProperty ActualBackProperty = DependencyProperty.Register(
            "ActualBack", typeof(string), typeof(Razdel_Page), new PropertyMetadata(default(string)));

        public string ActualBack
        {
            get { return (string) GetValue(ActualBackProperty); }
            set { SetValue(ActualBackProperty, value); }
        }

        public void GetBooks(List<BookPDF> actualBooks,string razdel)
        {
            ActualBooks = new ObservableCollection<BookPDF>();

          
            foreach (var actualBook in App.CurrentApp.ActualBooks)
            {
                if(actualBook.Icon.Source==null)
                actualBook.Icon=new DisposableImage(Directory.GetFiles(Directory.GetCurrentDirectory()+"\\Temp\\"+actualBook.Title)[0]);
               ActualBooks.Add(actualBook);
               
            }
        }

        private ICommand _bookCommand;

        public ICommand BookCommand =>
            _bookCommand ?? (_bookCommand = new Command(c =>
            {
               // App.CurrentApp.IsLoading = true;
                //int a = int.Parse(c.ToString())+1;
                NavigationService?.Navigate(new Book_Page(c.ToString()));

                GC.Collect();

            }));

        public static readonly DependencyProperty ActualBooksProperty = DependencyProperty.Register(
            "ActualBooks", typeof(ObservableCollection<BookPDF>), typeof(Razdel_Page), new PropertyMetadata(default(ObservableCollection<BookPDF>)));

        public ObservableCollection<BookPDF> ActualBooks
        {
            get { return (ObservableCollection<BookPDF>) GetValue(ActualBooksProperty); }
            set { SetValue(ActualBooksProperty, value); }
        }

        private ICommand _backCommand;

        public ICommand BackCommand =>
            _backCommand ?? (_backCommand = new Command(c =>
            {
                NavigationService?.Navigate(new Start_Page());
                GC.Collect();

            }));

        public static readonly DependencyProperty ActualRazdelProperty = DependencyProperty.Register(
            "ActualRazdel", typeof(string), typeof(Razdel_Page), new PropertyMetadata(default(string)));

        public string ActualRazdel
        {
            get { return (string) GetValue(ActualRazdelProperty); }
            set { SetValue(ActualRazdelProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            if(ActualBooks.Count==0)
            {
                foreach (var actualBook in App.CurrentApp.ActualBooks)
                {
                    if (actualBook.Icon.Source == null)
                        actualBook.Icon =
                            new DisposableImage(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Temp\\" +
                                                                   actualBook.Title)[0]);
                    ActualBooks.Add(actualBook);

                }
            }
        }
    }
}
