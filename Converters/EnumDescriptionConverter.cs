using System.Globalization;
using System.Windows.Data;
using USProApplication.DataBase.Entities;
using USProApplication.Utils;

namespace USProApplication.Converters;

public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Enum enumValue)
        {
            return enumValue.GetDescription();
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string description)
        {
            return EnumExtensions.GetValueFromDescription<DirectorPositions>(description);
        }
        throw new InvalidOperationException("Invalid conversion");
    }
}