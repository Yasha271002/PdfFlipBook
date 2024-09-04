using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfFlipBook.Annotations;
using PdfFlipBook.Models;

namespace PdfFlipBook.ViewModel.Pages
{
    public class SettingsPageViewModel:INotifyPropertyChanged
    {
        [CanBeNull] 
        private SettingsModel _settingsModel;

        [CanBeNull]
        public SettingsModel SettingsModel
        {
            get => _settingsModel;
            set
            {
                _settingsModel = value;
                OnPropertyChanged();
            }
        }

        public SettingsPageViewModel()
        {

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
