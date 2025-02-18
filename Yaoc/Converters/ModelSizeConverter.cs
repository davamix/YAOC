using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Yaoc.Converters;

public class ModelSizeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value == null) return "0B";

        double size = (Int64)value;

        var mb = size / 1024.0 / 1024.0;

        if (mb < 1024) return $"{mb:0.00}MB";

        return $"{mb / 1024.0:0.00}GB";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
