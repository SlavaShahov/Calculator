using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace UniversalCaluclator.Converters;

public class MemoryToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string memoryState = value?.ToString() ?? "";
        return memoryState == "M" ? new SolidColorBrush(Color.Parse("#4CAF50")) : new SolidColorBrush(Color.Parse("#E0E0E0"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}