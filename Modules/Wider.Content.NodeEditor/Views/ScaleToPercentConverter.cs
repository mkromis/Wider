using System;
using System.Globalization;
using System.Windows.Data;

namespace SampleCode
{
    /// <summary>
    /// Used in MainWindow.xaml to converts a scale value to a percentage.
    /// It is used to display the 50%, 100%, etc that appears underneath the zoom and pan control.
    /// </summary>
    public class ScaleToPercentConverter : IValueConverter
    {
        /// <summary>
        /// Convert a fraction to a percentage.
        /// <returns></returns>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            // Round to an integer value whilst converting.
            return (Double)(Int32)((Double)value * 100.0);
        }

        /// <summary>
        /// Convert a percentage back to a fraction.
        /// <returns></returns>
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) => (Double)value / 100.0;
    }
}
