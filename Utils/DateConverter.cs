namespace USProApplication.Utils
{
    public static class DateConverter
    {
        /// <summary>
        /// Преобразует дату в строку в зависимости от типа
        /// </summary>
        /// <param name="inputDate">Дата</param>
        /// <param name="formatType">Тип формата ("string" или "number")</param>
        /// <returns>Отформатированная строка</returns>
        public static string ConvertDateToString(DateTime? inputDate, string formatType = "string")
        {
            if (inputDate == null)
            {
                return string.Empty;
            }

            DateTime date = (DateTime)inputDate;

            if (formatType.Equals("string", StringComparison.CurrentCultureIgnoreCase))
            {
                // Формат вида «20» января 2024
                return $"«{date:dd}» {GetMonthName(date.Month)} {date:yyyy}";
            }
            else if (formatType.Equals("number", StringComparison.CurrentCultureIgnoreCase))
            {
                // Формат вида «20» 01 2024
                return $"«{date:dd}» {date:MM} {date:yyyy}";
            }
            else
            {
                throw new ArgumentException("Недопустимый тип формата. Используйте \"string\" или \"number\".");
            }
        }

        /// <summary>
        /// Возвращает название месяца на русском языке
        /// </summary>
        private static string GetMonthName(int month)
        {
            return month switch
            {
                1 => "января",
                2 => "февраля",
                3 => "марта",
                4 => "апреля",
                5 => "мая",
                6 => "июня",
                7 => "июля",
                8 => "августа",
                9 => "сентября",
                10 => "октября",
                11 => "ноября",
                12 => "декабря",
                _ => throw new ArgumentOutOfRangeException(nameof(month), "Месяц должен быть от 1 до 12")
            };
        }
    }
}
