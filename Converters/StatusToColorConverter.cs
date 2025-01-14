using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace USProApplication.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "Выполнен" => Brushes.Green,
                "Просрочен" => Brushes.Red,
                "В работе" => new SolidColorBrush(Color.FromRgb(23, 141, 191)),
                _ => Brushes.Black,
            };
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
