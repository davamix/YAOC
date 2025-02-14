using OllamaSharp.Models.Chat;
using System.Globalization;
using System.Windows.Data;

namespace Yaoc.Converters;
public class RoleToColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        ChatRole role = (ChatRole)value;
        
        return role.ToString() switch {
            "system" => "LightGray",
            "user" => "LightGreen",
            "assistant" => "LightBlue",
            "tool" => "LightYellow",
            _ => "White"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
