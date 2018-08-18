#region License
// Copyright (c) 2018 Mark Kromis
// Copyright (c) 2013 Chandramouleswaran Ravichandran
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Wider.Core.Converters
{
    /// <summary>
    /// Class PercentToFontSizeConverter
    /// </summary>
    public class PercentToFontSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            //For now lets assume 12.00 to be 100%
            Double? fsize = value as Double?;
            if (fsize != null)
            {
                return ((fsize/12.00)*100) + " %";
            }
            return "100 %";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            Double rValue = 12.0;
            if (value != null)
            {
                String final = value as String;
                final = final.Replace("%", "");
                if (Double.TryParse(final, out rValue))
                {
                    rValue = (rValue/100.0)*12;
                }
                else
                {
                    rValue = 12.0;
                }
            }
            return rValue;
        }

        #endregion
    }
}