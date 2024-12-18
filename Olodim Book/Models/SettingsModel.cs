using System;
using System.Windows.Media;
using Core;
using Newtonsoft.Json;
using PdfFlipBook.Helper;
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
        [JsonIgnore]
        private JsonHelper _jsonHelper;

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
        public int InactivityTimePage
        {
            get => GetOrCreate<int>(60);
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

        public bool OnTapSwitchPage
        {
            get => GetOrCreate<bool>();
            set => SetAndNotify(value);
        }

        public SelectThemes SelectedThemes
        {
            get => GetOrCreate<SelectThemes>();
            set => SetAndNotify(value);
        }

        public System.Windows.Media.Brush SelectedBrush
        {
            get => GetOrCreate<Brush>();
            set => SetAndNotify(value);
        }

        public System.Windows.Media.Color SelectedColor
        {
            get => GetOrCreate<Color>();
            set => SetAndNotify(value);
        }

        public bool IsDarkTheme
        {
            get => GetOrCreate<bool>();
            set
            {
                SetAndNotify(value);
                SwitchThemes();
            }
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

        public double Hue
        {
            get => GetOrCreate<double>();
            set
            {
                SetAndNotify(value);
                UpdateColor();
                GetColorFromHueWithOffset(value, 1);
            }
        }
        public double Saturation
        {
            get => GetOrCreate<double>();
            set
            {
                SetAndNotify(value);
                UpdateColor();
            }
        }
        public double Brightness
        {
            get => GetOrCreate<double>();
            set
            {
                SetAndNotify(value);
                UpdateColor();
            }
        }
        public Color HueBrush
        {
            get => GetOrCreate<Color>();
            set => SetAndNotify(value);
        }

        [JsonIgnore]
        public double JsonHue
        {
            get => GetOrCreate<double>();
            set
            {
                SetAndNotify(value);
                UpdateColor();
                GetColorFromHueWithOffset(value, 1);
            }
        }
        [JsonIgnore]
        public double JsonSaturation
        {
            get => GetOrCreate<double>();
            set
            {
                SetAndNotify(value);
                UpdateColor();
            }
        }
        [JsonIgnore]
        public double JsonBrightness
        {
            get => GetOrCreate<double>();
            set
            {
                SetAndNotify(value);
                UpdateColor();
            }
        }
        [JsonIgnore]
        public Color JsonHueBrush
        {
            get => GetOrCreate<Color>();
            set => SetAndNotify(value);
        }
        [JsonIgnore]
        public Brush JsonBrush
        {
            get => GetOrCreate<Brush>();
            set => SetAndNotify(value);
        }
        [JsonIgnore]
        public Color JsonColor
        {
            get => GetOrCreate<Color>();
            set => SetAndNotify(value);
        }

        #region ColorUpdate

        private void UpdateColor()
        {
            SelectedColor = ConvertHsbToRgb(Hue, Saturation, Brightness);
            SelectedBrush = new SolidColorBrush(SelectedColor);

            HueBrush = GetColorFromHueWithOffset(Hue, 1);
        }

        private static System.Windows.Media.Color ConvertHsbToRgb(double hue, double saturation, double brightness)
        {
            var c = brightness * saturation;
            var x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            var m = brightness - c;

            double r = 0, g = 0, b = 0;
            switch (hue)
            {
                case < 60:
                    r = c; g = x; b = 0;
                    break;
                case < 120:
                    r = x; g = c; b = 0;
                    break;
                case < 180:
                    r = 0; g = c; b = x;
                    break;
                case < 240:
                    r = 0; g = x; b = c;
                    break;
                case < 300:
                    r = x; g = 0; b = c;
                    break;
                default:
                    r = c; g = 0; b = x;
                    break;
            }

            return System.Windows.Media.Color.FromScRgb(1.0f, (float)(r + m), (float)(g + m), (float)(b + m));
        }

        public System.Windows.Media.Color GetColorFromHueWithOffset(double hue, double offset)
        {
            hue += offset;
            hue = hue % 360;
            if (hue < 0) hue += 360;
            return ConvertHsbToRgb(hue, 1.0, 1.0);
        }

        private void SwitchThemes()
        {
            _jsonHelper = new JsonHelper();
            var jsonThemesPath = "Themes.json";
            var dark = "Dark";
            var light = "Light";

            if (IsDarkTheme)
            {
                App.CurrentApp.ChangeTheme(dark);
                _jsonHelper.WriteJsonToFile(jsonThemesPath, dark, false);

                Hue = 240;
                Brightness = 0.025;
                Saturation = 0.002;
            }
            else
            {
                App.CurrentApp.ChangeTheme(light);
                _jsonHelper.WriteJsonToFile(jsonThemesPath, light, false);

                Hue = 0;
                Brightness = 100;
                Saturation = 0;

            }
        }

        #endregion
    }
}
