using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfFlipBook.Utilities;

namespace PdfFlipBook.Models
{
   public class BookPDF:INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Book { get; set; }
        public string FullPath { get; set; }
        public string Text { get; set; }
        public DisposableImage Icon { get; set; }

        private double _borderWidth;
        public double BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                OnPropertyChanged();
            }
        }

        private double _borderHeight;
        public double BorderHeight
        {
            get => _borderHeight;
            set
            {
                _borderHeight = value;
                OnPropertyChanged();
            }
        }

        private double _imageWidth;
        public double ImageWidth
        {
            get => _imageWidth;
            set
            {
                _imageWidth = value;
                OnPropertyChanged();
            }
        }

        private double _imageHeight;
        public double ImageHeight
        {
            get => _imageHeight;
            set
            {
                _imageHeight = value;
                OnPropertyChanged();
            }
        }

        private double _scrollViewerHeight;
        public double ScrollViewerHeight
        {
            get => _scrollViewerHeight;
            set
            {
                _scrollViewerHeight = value;
                OnPropertyChanged();
            }
        }

        private double _wrapPanelWidth;
        public double WrapPanelWidth
        {
            get => _wrapPanelWidth;
            set
            {
                _wrapPanelWidth = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
