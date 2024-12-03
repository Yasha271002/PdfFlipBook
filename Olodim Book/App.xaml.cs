using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using PdfFlipBook.Helper.Logger;
using PdfFlipBook.Models;
using PdfFlipBook.Properties;
using PdfFlipBook.Views.Pages;

namespace PdfFlipBook
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application,INotifyPropertyChanged
    {
        public static App CurrentApp => App.Current as App;

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Instance._logger.Error(e.Exception.Message);
            e.Handled = true;
        }

        private bool _isLoading;

        private List<BookPDF> _actualBooks;
        public List<BookPDF> ActualBooks
        {
            get => _actualBooks ?? (_actualBooks = new List<BookPDF>());
            set
            {
                _actualBooks = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<BookPDF> _allBooks;
        public ObservableCollection<BookPDF> AllBooks
        {
            get => _allBooks ?? (_allBooks = new ObservableCollection<BookPDF>());
            set
            {
                _allBooks = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        private string _background;

        public string Background
        {
            get => _background;
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }
        private bool _secondTime;

        public bool SecondTime
        {
            get => _secondTime;
            set
            {
                _secondTime = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
