using System;
using System.Collections.ObjectModel;
using PdfFlipBook.Models;

namespace PdfFlipBook.Helper.Singleton
{
    public class GlobalSettings
    {
        private static readonly Lazy<GlobalSettings> _instance = new Lazy<GlobalSettings>(() => new GlobalSettings());

        public static GlobalSettings Instance => _instance.Value;
        public ObservableCollection<BookPDF> Books { get; set; } = new ObservableCollection<BookPDF>();
        public SettingsModel Settings { get; set; }

        private GlobalSettings()
        {
        }

    }
}
