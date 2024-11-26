using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PdfFlipBook.Helper.Converters
{
    public class ImageConverter : MarkupExtension, IValueConverter
    {
        public int? Width { get; set; }
        public int? Height { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is not string path ? null : ImageHelper.GetImage(path, Width, Height);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
