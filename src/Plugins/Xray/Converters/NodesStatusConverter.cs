﻿using Avalonia.Data.Converters;
using System.Globalization;

namespace Xray.Converters;

public sealed class NodesStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not XrayNodeStatus status)
        {
            return null;
        }
        return (int)status == System.Convert.ToInt32(parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}