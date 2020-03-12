using System;
using System.Globalization;
using System.Windows.Data;

namespace Arduchuni.Server {
    [ValueConversion(typeof(bool), typeof(object))]
    public sealed class BooleanConverter : IValueConverter {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        public bool Default { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is bool b)) {
                b = Default;
            }

            return b ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
