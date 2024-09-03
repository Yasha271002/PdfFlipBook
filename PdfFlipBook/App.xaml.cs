using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using PdfFlipBook.Annotations;
using PdfFlipBook.Models;
using PdfFlipBook.Views;

namespace PdfFlipBook
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application,INotifyPropertyChanged
    {
        
        public static App CurrentApp => App.Current as App;

       
        public Start_Page SP = new Start_Page();
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

        private List<BookPDF> _allBooks;
        public List<BookPDF> AllBooks
        {
            get => _allBooks ?? (_allBooks = new List<BookPDF>());
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
