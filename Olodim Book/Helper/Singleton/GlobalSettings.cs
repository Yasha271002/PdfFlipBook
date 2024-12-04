using System;
using System.Collections.ObjectModel;
using Core;
using PdfFlipBook.Models;

namespace PdfFlipBook.Helper.Singleton
{
    public class GlobalSettings : ObservableObject
    {
        private static readonly Lazy<GlobalSettings> _lazyInstance = new Lazy<GlobalSettings>(() => new GlobalSettings());

        public static GlobalSettings Instance => _lazyInstance.Value;

        public ObservableCollection<BookPDF> Books { get; set; } = new ObservableCollection<BookPDF>();

        private SettingsModel _settings;
        public SettingsModel Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new SettingsModel();
                }
                return _settings;
            }
            set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        private GlobalSettings()
        {
        }
    }
}

