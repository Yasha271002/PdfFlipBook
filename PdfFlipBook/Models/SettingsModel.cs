using Core;
using PdfFlipBook.Annotations;

namespace PdfFlipBook.Models
{
    public class SettingsModel:ObservableObject
    {
        [CanBeNull]
        public string Password
        {
            get => GetOrCreate<string>(); 
            set => SetAndNotify(value);
        }

        [CanBeNull]
        public string InactivityTime
        {
            get => GetOrCreate<string>();
            set => SetAndNotify(value);
        }

        [CanBeNull]
        public string IntervalSwitchPage
        {
            get => GetOrCreate<string>();
            set => SetAndNotify(value);
        }

        public bool Repeat
        {
            get => GetOrCreate<bool>();
            set => SetAndNotify(value); 
        }

        public bool NextPage
        {
            get => GetOrCreate<bool>();
            set => SetAndNotify(value);
        }
    }
}
