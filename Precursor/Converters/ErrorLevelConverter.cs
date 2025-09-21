using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Precursor.Converters
{
    public class ErrorLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo) 
        {
            if (value is string errorLevel) 
            {
                return (errorLevel?.ToLower()) switch
                {
                    "none" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d4edda")),
                    "intermediate" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffebbf")),
                    "all" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffe6e6")),
                    _ => new SolidColorBrush(Colors.White),
                };
            }

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo) 
        {
            throw new NotImplementedException();
        }
    }
}
