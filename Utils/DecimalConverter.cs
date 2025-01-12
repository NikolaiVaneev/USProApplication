namespace USProApplication.Utils;

public static class DecimalConverter
{
    private static readonly string[] Units =
    {
    "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"
};

    private static readonly string[] Teens =
    {
    "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать",
    "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"
};

    private static readonly string[] Tens =
    {
    "", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"
};

    private static readonly string[] Hundreds =
    {
    "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"
};

    private static readonly string[] RublesForms = { "рубль", "рубля", "рублей" };
    private static readonly string[] KopecksForms = { "копейка", "копейки", "копеек" };

    public static string ConvertDecimalToString(decimal? amount)
    {
        if (amount == null)
            return string.Empty;

        int rubles = (int)amount;
        int kopecks = (int)((amount - rubles) * 100);

        return $"{ConvertNumberToString(rubles, RublesForms)} {kopecks:00} коп.";
    }

    private static string ConvertNumberToString(int number, string[] forms)
    {
        if (number == 0)
            return "ноль " + forms[2];

        string result = "";

        // Разбивка на миллионы, тысячи и сотни
        int millions = number / 1_000_000;
        int thousands = (number % 1_000_000) / 1_000;
        int rest = number % 1_000;

        if (millions > 0)
            result += $"{ConvertNumberGroupToString(millions, new[] { "миллион", "миллиона", "миллионов" })} ";

        if (thousands > 0)
            result += $"{ConvertNumberGroupToString(thousands, new[] { "тысяча", "тысячи", "тысяч" }, isFeminine: true)} ";

        if (rest > 0)
        {
            result += $"{ConvertNumberGroupToString(rest, forms)}";
        }
        else
        {
            result += forms[2]; // Добавляем "рублей", если остаток 0
        }

        return result.Trim();
    }

    private static string ConvertNumberGroupToString(int number, string[] forms, bool isFeminine = false)
    {
        string[] units = isFeminine ? ["", "одна", "две"] : ["", "один", "два"];

        string result = "";
        int hundreds = number / 100;
        int tensUnits = number % 100;
        int tens = tensUnits / 10;
        int unitsDigit = tensUnits % 10;

        if (hundreds > 0)
            result += Hundreds[hundreds] + " ";

        if (tensUnits > 9 && tensUnits < 20)
        {
            result += Teens[tensUnits - 10] + " ";
        }
        else
        {
            if (tens > 0)
                result += Tens[tens] + " ";

            if (unitsDigit > 0)
                result += (unitsDigit <= 2 ? units[unitsDigit] : Units[unitsDigit]) + " ";
        }

        result = result.Trim();
        result += " " + GetForm(number, forms);

        return result.Trim();
    }

    private static string GetForm(int number, string[] forms)
    {
        int lastDigit = number % 10;
        int lastTwoDigits = number % 100;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
            return forms[2]; // например, "рублей"
        else if (lastDigit == 1)
            return forms[0]; // например, "рубль"
        else if (lastDigit >= 2 && lastDigit <= 4)
            return forms[1]; // например, "рубля"
        else
            return forms[2]; // например, "рублей"
    }
}
