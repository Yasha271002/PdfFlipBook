using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using PdfFlipBook.Annotations;

namespace PdfFlipBook.Models
{
    public class PageSettingsModel : ObservableObject
    {
        [CanBeNull]
        public string Password
        {
            get => GetOrCreate(string.Empty);
            set => SetAndNotify(value);
        }

        public bool CorrectPassword
        {
            get => GetOrCreate(true);
            set => SetAndNotify(value);
        }

        public bool ShowPassword
        {
            get => GetOrCreate(false);
            set => SetAndNotify(value);
        }

        public bool ShowError
        {
            get => GetOrCreate(false);
            set => SetAndNotify(value);
        }

        public bool ShowSettings
        {
            get => GetOrCreate(false);
            set => SetAndNotify(value);
        }

        public bool IsPinPadVisible
        {
            get => GetOrCreate(false);
            set => SetAndNotify(value);
        }

        [CanBeNull]
        public string Type
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
        public string Interval
        {
            get => GetOrCreate<string>();
            set => SetAndNotify(value);
        }
    }
}
