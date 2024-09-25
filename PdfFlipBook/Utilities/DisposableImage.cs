using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using PdfFlipBook.Annotations;


namespace PdfFlipBook.Utilities
{
    public class DisposableImage : IDisposable,INotifyPropertyChanged
    {
        /// <summary>
        /// Уничтожаемая картинка
        /// </summary>
        /// <param name="path">Путь до картинки</param>
        public DisposableImage(string path = null, Stream str = null)
        {
            Update(path,str);
        }
        Stream mediaStream;

        private BitmapImage _source;
        /// <summary>
        /// Картинка
        /// </summary>
        public BitmapImage Source
        {
            get => _source;
            private set
            {
                _source = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Добавляем картинку
        /// </summary>
        /// <param name="path">путь</param>
        private void Update(string path, Stream str)
        {
            Dispose();

            var bitmap = new BitmapImage();
            if (path==null&&str!=null)
            {
                mediaStream = str;
            }
            else
            {
                if (path.Contains("pack://application:,,,"))
                {
                    mediaStream = Application.GetResourceStream(new Uri(path))?.Stream;
                }
                else
                {
                    mediaStream = File.OpenRead(path);
                }
            }
            


            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.StreamSource = mediaStream;
            bitmap.EndInit();

            bitmap.Freeze();
            Source = bitmap;
        }





        /// <summary>
        /// Уничтожить картинку
        /// </summary>
        public void Dispose()
        {
            if (mediaStream != null)
            {
                Source = null;
                mediaStream.Close();
                mediaStream.Dispose();
                mediaStream = null;

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
