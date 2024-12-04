﻿using System.Windows.Media;
using Core;
using PdfFlipBook.Properties;

namespace PdfFlipBook.Models
{
    public enum SelectThemes
    {
        Dark,
        Light,
        Custom
    }

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

        public float Volume
        {
            get => GetOrCreate(0.5f);
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

        public SelectThemes SelectedThemes
        {
            get => GetOrCreate<SelectThemes>();
            set => SetAndNotify(value);
        }

        public SolidColorBrush SelectedBrush
        {
            get => GetOrCreate<SolidColorBrush>();
            set => SetAndNotify(value);
        }

        public System.Windows.Media.Color SelectedColor
        {
            get => GetOrCreate<Color>();
            set => SetAndNotify(value);
        }

        public string? MainBackgroundSoundPath
        {
            get => GetOrCreate<string>();
            set => SetAndNotify(value);
        }

        public string? SwitchSoundPath
        {
            get => GetOrCreate<string>();
            set => SetAndNotify(value);
        }
    }
}
