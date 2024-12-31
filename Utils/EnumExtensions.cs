using System.ComponentModel;

namespace USProApplication.Utils;

public static class EnumExtensions
{
    public static IEnumerable<string> GetDescriptions<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
                   .Cast<T>()
                   .Select(e => e.GetDescription());
    }

    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                              .Cast<DescriptionAttribute>()
                              .FirstOrDefault();
        return attribute?.Description ?? value.ToString();
    }

    public static T GetValueFromDescription<T>(string description) where T : Enum
    {
        var type = typeof(T);
        foreach (var field in type.GetFields())
        {
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                  .Cast<DescriptionAttribute>()
                                  .FirstOrDefault();
            if ((attribute?.Description ?? field.Name) == description)
            {
                return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException($"Не найдено значение для описания '{description}'", nameof(description));
    }
}