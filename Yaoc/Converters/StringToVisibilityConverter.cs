using System.Globalization;
using System.Windows.Data;

namespace Yaoc.Converters;
public class StringToVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if(string.IsNullOrEmpty((string)value)) {
            return System.Windows.Visibility.Collapsed;
        }

        return System.Windows.Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
