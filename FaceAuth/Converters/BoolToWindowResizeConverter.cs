using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FaceAuth.Converters
{
    public class BoolToWindowResizeConverter: BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value.GetType() != typeof(bool))
                throw new ArgumentException("takes only bool");
            var input = (bool) value;
            return input ? "CanResize" : "NoResize";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(string))
                throw new ArgumentException("takes only strings");
            var input = (string) value;
            return input == "CanResize";
        }
    }
}
