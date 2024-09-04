using System.Windows;
using System.Windows.Controls;
using PdfFlipBook.Models;

namespace PdfFlipBook.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public static readonly DependencyProperty SettingsModelProperty = DependencyProperty.Register(
            nameof(SettingsModel), typeof(SettingsModel), typeof(SettingsControl), new PropertyMetadata(default(SettingsModel)));

        public SettingsModel SettingsModel
        {
            get { return (SettingsModel)GetValue(SettingsModelProperty); }
            set { SetValue(SettingsModelProperty, value); }
        }

        public SettingsControl()
        {
            InitializeComponent();
        }
    }
}
