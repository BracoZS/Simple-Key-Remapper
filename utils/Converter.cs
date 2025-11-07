using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Simple_Key_Remapper.utils;
public class EnumtoKeyUI : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value == null)
            return string.Empty;

        return value.ToString()!.Replace("Vc", ""); //KeyCode to string removing "Vc" prefix
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
