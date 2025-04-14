using System.Globalization;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Converters
{
    public class ConditionalUnderlineConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is string text)
            {
                if (text.SequenceEqual(AppResources.NotAvailable)) return TextDecorations.None;
                else return TextDecorations.Underline;
            }
            return TextDecorations.Underline;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
