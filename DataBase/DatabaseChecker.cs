using System.IO;

namespace USProApplication.DataBase;

public static class DatabaseChecker
{
    public static void EnsureDatabaseExists()
    {
        var connectionString = AppConfiguration.GetConnectionString("DefaultConnection");
        var databasePath = GetDatabasePathFromConnectionString(connectionString);

        if (!File.Exists(databasePath))
        {
           // CreateDatabase(databasePath);
        }
    }

    private static string GetDatabasePathFromConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Отсутствуют настройки подключения к базе данных.");
        }

        const string dataSourceKey = "Data Source=";
        var startIndex = connectionString.IndexOf(dataSourceKey, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            throw new InvalidOperationException("Некорректная строка подключения к базе данных.");
        }

        startIndex += dataSourceKey.Length;
        var endIndex = connectionString.IndexOf(';', startIndex);
        return endIndex == -1 ? connectionString[startIndex..] : connectionString[startIndex..endIndex];
    }

    //private static void CreateDatabase(string databasePath)
    //{
    //    using var context = new AppDbContext();
    //    context.Database.EnsureCreated();
    //}
}