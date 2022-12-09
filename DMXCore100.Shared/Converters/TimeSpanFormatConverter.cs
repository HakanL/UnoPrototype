using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace DMXCore.DMXCore100
{
    public class TimeSpanFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = value switch
            {
                TimeSpan timeSpanValue => timeSpanValue.ToString("h\\:mm\\:ss\\.fff"),
                _ => DependencyProperty.UnsetValue
            };

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
