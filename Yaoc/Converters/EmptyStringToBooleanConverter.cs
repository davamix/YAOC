using System.Globalization;
using System.Windows.Data;

namespace Yaoc.Converters; 
public class EmptyStringToBooleanConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var b = (string)value;

        return !string.IsNullOrEmpty(b);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
