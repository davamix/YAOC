﻿using System.Globalization;
using System.Windows.Data;

namespace Yaoc.Converters; 
public class BooleanToVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        bool isVisible = (bool)value;

        if (isVisible) {
            return System.Windows.Visibility.Visible;
        }

        return System.Windows.Visibility.Collapsed;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
