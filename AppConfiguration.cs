using Microsoft.Extensions.Configuration;

namespace USProApplication;

public static class AppConfiguration
{
    public static IConfigurationRoot Configuration { get; }

    static AppConfiguration()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public static string? GetConnectionString(string name)
    {
        return Configuration.GetConnectionString(name);
    }
}
