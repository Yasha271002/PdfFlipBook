using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using Core;
using PdfFlipBook.Helper;
using PdfFlipBook.Helper.Singleton;
using PdfFlipBook.Models;

namespace PdfFlipBook.ViewModel.Pages
{
    public class SettingsPageViewModel : ObservableObject
    {
        DispatcherTimer _timer = new DispatcherTimer();
        private int _sec = 0;

        public SettingsModel SettingsModel
        {
            get => GetOrCreate<SettingsModel>();
            set => SetAndNotify(value);
        }

        public PageSettingsModel PageSettings
        {
            get => GetOrCreate<PageSettingsModel>();
            set => SetAndNotify(value);
        }

        public SolidColorBrush SelectedBrush
        {
            get => GetOrCreate<SolidColorBrush>();
            set => SetAndNotify(value);
        }

        public System.Windows.Media.Color SelectedColor
        {
            get => GetOrCreate<System.Windows.Media.Color>();
            set
            {
                SetAndNotify(value);
                SelectedBrush = new SolidColorBrush(value);
            }
        }

        public SettingsPageViewModel(SettingsModel settings)
        {
            SettingsModel = settings;
            StartPageSettings();
        }

        private ICommand _checkPasswordCommand;

        public ICommand CheckPasswordCommand => _checkPasswordCommand ??= new RelayCommand(f =>
        {
            if (f is not string password) return;
            CheckPassword(password);
        });

        public ICommand ShowPasswordCommand => GetOrCreate(new RelayCommand(f =>
        {
            PageSettings.ShowPassword = !PageSettings.ShowPassword;
        }));

        public ICommand ButtonPinPadCommand => GetOrCreate(new RelayCommand(OnPinPadButtonPressed));

        public ICommand GotFocusCommand => GetOrCreate(new RelayCommand(OnGotFocus));

        public ICommand LostFocusCommand => GetOrCreate(new RelayCommand(OnLostFocus));

        public ICommand BackPinPad => GetOrCreate(new RelayCommand(f =>
        {
            if (!string.IsNullOrEmpty(PageSettings.Password))
                PageSettings.Password = PageSettings.Password.Substring(0, PageSettings.Password.Length - 1);

            OnPropertyChanged(nameof(PageSettings.Password));
        }));

        public ICommand EditSettingsCommand => GetOrCreate(new RelayCommand(f =>
        {
            if (f is not string type) return;
            EditSettings(type);
        }));

        public ICommand WriteJsonFileAndBackCommand => GetOrCreate(new RelayCommand(f =>
        {
            var helper = new JsonHelper();
            var filePath = "Settings/Settings.json";
            helper.WriteJsonToFile(filePath, SettingsModel, false);
            CommonCommands.GoBackCommand!.Execute(null);
        }));

        private void EditSettings(string type)
        {
            if (type == "Edit")
            {
                switch (PageSettings.Type)
                {
                    case "Inactivity":
                        SettingsModel.InactivityTime = PageSettings.InactivityTime;
                        PageSettings.InactivityTime = string.Empty;
                        OnPropertyChanged(nameof(SettingsModel.InactivityTime));
                        break;
                    case "Interval":
                        SettingsModel.IntervalSwitchPage = PageSettings.Interval;
                        PageSettings.Interval = string.Empty;
                        OnPropertyChanged(nameof(SettingsModel.IntervalSwitchPage));
                        break;
                }
            }
            else
            {
                switch (PageSettings.Type)
                {
                    case "Password":
                        PageSettings.Password = "";
                        OnPropertyChanged(nameof(PageSettings.Password));
                        break;
                    case "Inactivity":
                        PageSettings.InactivityTime = "";
                        OnPropertyChanged(nameof(PageSettings.InactivityTime));
                        break;
                    case "Interval":
                        PageSettings.Interval = "";
                        OnPropertyChanged(nameof(PageSettings.Interval));
                        break;
                }
            }
        }

        private void CheckPassword(string password)
        {
            if (password is null) return;

            PageSettings.IsPinPadVisible = false;

            if (SettingsModel.Password == password)
            {
                PageSettings.CorrectPassword = true;
                PageSettings.ShowError = false;
                PageSettings.ShowSettings = true;
            }
            else
            {
                PageSettings.ShowPassword = false;
                PageSettings.CorrectPassword = false;
                PageSettings.ShowError = true;
                PageSettings.Password = string.Empty;

                _timer?.Stop();
                _sec = 0;
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += Timer;
                _timer.Start();
            }
        }

        private void Timer(object sender, EventArgs eventArgs)
        {
            _sec++;
            if (_sec < 3) return;
            PageSettings.ShowError = false;
            _timer.Stop();
        }

        private void OnGotFocus(object parameter)
        {
            parameter ??= PageSettings.Type;

            if (parameter is not string type) return;
            PageSettings.Type = type;
            PageSettings.IsPinPadVisible = true;
        }

        private void OnLostFocus(object parameter)
        {
            PageSettings.IsPinPadVisible = false;
        }

        private void OnPinPadButtonPressed(object parameter)
        {
            if (parameter is not string value) return;

            switch (PageSettings.Type)
            {
                case "Password":
                    PageSettings.Password += value;
                    OnPropertyChanged(nameof(PageSettings.Password));
                    break;
                case "Inactivity":
                    PageSettings.InactivityTime += value;
                    OnPropertyChanged(nameof(PageSettings.InactivityTime));
                    break;
                case "Interval":
                    PageSettings.Interval += value;
                    OnPropertyChanged(nameof(PageSettings.Interval));
                    break;
            }
        }

        private void StartPageSettings()
        {
            PageSettings = new PageSettingsModel
            {
                IsPinPadVisible = false,
                Password = string.Empty,
                CorrectPassword = false,
                ShowError = false,
                ShowPassword = false,
                ShowSettings = false
            };
        }
    }
}