using System.Globalization;
using System.Windows.Data;

namespace Yaoc.Converters;
public class ConversationToVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if(value != null) return System.Windows.Visibility.Visible;

        return System.Windows.Visibility.Collapsed;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
