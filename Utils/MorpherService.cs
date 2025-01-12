using System.Net.Http;
using System.Text.Json;

namespace USProApplication.Utils;

public class MorpherService
{
    private readonly string _baseUrl = "https://ws3.morpher.ru/russian/declension";

    /// <summary>
    /// Перечисление падежей на английском языке.
    /// </summary>
    public enum RussianCase
    {
        Nominative,   // Именительный (Nom)
        Genitive,     // Родительный (Gen)
        Dative,       // Дательный (Dat)
        Accusative,   // Винительный (Acc)
        Instrumental, // Творительный (Instr)
        Prepositional // Предложный (Prep)
    }

    /// <summary>
    /// Получает строку в заданном падеже.
    /// </summary>
    /// <param name="fullName">Фамилия Имя Отчество.</param>
    /// <param name="caseName">Падеж в виде перечисления.</param>
    /// <returns>Строка ФИО в указанном падеже.</returns>
    public async Task<string> GetDeclensionAsync(string? fullName, RussianCase caseName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return string.Empty;
        }

        using var client = new HttpClient();
        var requestUrl = $"{_baseUrl}?s={Uri.EscapeDataString(fullName)}&format=json";

        try
        {
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MorpherResponse>(jsonResponse);

            if (result != null)
            {
                return caseName switch
                {
                    RussianCase.Nominative => fullName,  // Именительный падеж возвращаем как есть
                    RussianCase.Genitive => result.Р,
                    RussianCase.Dative => result.Д,
                    RussianCase.Accusative => result.В,
                    RussianCase.Instrumental => result.Т,
                    RussianCase.Prepositional => result.П
                };
            }
            else
            {
                return fullName;
            }
        }
        catch (Exception)
        {
            return fullName;
        }
    }

    public async Task<string> GetShortNameAsync(string? fullName, RussianCase caseName)
    {
        var result = await GetDeclensionAsync(fullName, caseName);

        // Разбиваем строку на части
        var nameParts = result?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (nameParts == null || nameParts.Length < 3)
        {
            throw new Exception("Некорректный формат ФИО.");
        }

        string lastName = nameParts[0]; // Фамилия
        string? firstName = nameParts[1]; // Имя
        string? patronymic = nameParts[2]; // Отчество

        // Формируем итоговую строку
        string initials = string.Empty;
        if (!string.IsNullOrWhiteSpace(firstName))
        {
            initials += firstName[0] + ".";
        }
        if (!string.IsNullOrWhiteSpace(patronymic))
        {
            initials += patronymic[0] + ".";
        }

        return $"{lastName} {initials}";
    }

    // Класс для десериализации JSON-ответа
    private class MorpherResponse
    {
        public string? Р { get; set; } 
        public string? Д { get; set; }
        public string? В { get; set; }
        public string? Т { get; set; }
        public string? П { get; set; }
        public Fio? ФИО { get; set; }

        public class Fio
        {
            public string? Ф { get; set; }
            public string? И { get; set; }
            public string? О { get; set; }
        }
    }
}