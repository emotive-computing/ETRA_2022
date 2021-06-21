using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GazeTracking4C
{

    /// <summary>
    /// Data converter for scaling floating-point values.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Invoked from Xaml.")]
    public class ScalingConverter : IMultiValueConverter
    {
        public ScalingConverter()
        {

        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return null;
            }

            double doubleParam;
            var stringParam = parameter as string;
            if (values.Length == 1 &&
                values[0] is double &&
                stringParam != null &&
                double.TryParse(stringParam, out doubleParam))
            {
                return (double)values[0] * doubleParam;
            }
            else if (values.Length == 2 &&
                values[0] is double &&
                values[1] is double)
            {
                return (double)values[0] * (double)values[1];
            }
            else if (values.Length == 3 &&
                values[0] is Point &&
                values[1] is double &&
                values[2] is double)
            {
                var point = (Point)values[0];
                return new Point(point.X * (double)values[1], point.Y * (double)values[2]);
            }
            else
            {
                return 0.0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
