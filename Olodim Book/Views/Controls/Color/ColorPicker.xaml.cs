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

        

        
    }
}
