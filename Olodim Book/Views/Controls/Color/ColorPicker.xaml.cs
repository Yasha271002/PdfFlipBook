using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PdfFlipBook.Models;

namespace PdfFlipBook.Views.Controls.Color
{
    /// <summary>
    /// Логика взаимодействия для ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            nameof(SelectedColor), typeof(System.Windows.Media.Color), typeof(ColorPicker),
            new PropertyMetadata(Colors.Black, OnColorChanged));

        public System.Windows.Media.Color SelectedColor
        {
            get => (System.Windows.Media.Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            nameof(SelectedBrush), typeof(SolidColorBrush), typeof(ColorPicker),
            new PropertyMetadata(Brushes.Black));

        public SolidColorBrush SelectedBrush
        {
            get => (SolidColorBrush)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        public double Hue
        {
            get => _hue;
            set
            {
                _hue = value;
                UpdateColor();
            }
        }
        public double Saturation
        {
            get => _saturation;
            set
            {
                _saturation = value;
                UpdateColor();
            }
        }
        public double Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                UpdateColor();
            }
        }

        private double _hue;
        private double _saturation;
        private double _brightness;

        public ColorPicker()
        {
            InitializeComponent();
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPicker picker || e.NewValue is not System.Windows.Media.Color newColor) return;
            if (!picker.SelectedColor.Equals(newColor))
            {
                picker.SelectedBrush = new SolidColorBrush(newColor);
            }
        }

        private void UpdateColor()
        {
            SelectedColor = ConvertHsbToRgb(Hue, Saturation, Brightness);
            SelectedBrush = new SolidColorBrush(SelectedColor);
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
    }
}
