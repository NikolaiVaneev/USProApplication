using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using USProApplication.DataBase;
using USProApplication.DataBase.Mappings;
using USProApplication.DataBase.Repository;
using USProApplication.Models;
using USProApplication.ViewModels;

namespace USProApplication;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public App()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextFactory<AppDbContext>(options =>
        {
            options.UseSqlite(AppConfiguration.GetConnectionString("DefaultConnection"));
        });

        // Регистрация AutoMapper
        services.AddAutoMapper(typeof(ServiceMap));

        services.AddScoped<IBaseRepository<Service>, ServicesRepository>();
        services.AddScoped<ServicesViewModel>();
        services.AddDbContext<AppDbContext>();
        
        // Добавляйте другие зависимости здесь
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Установка культуры "ru-RU" глобально для всего приложения
        var culture = new CultureInfo("ru-RU");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        try
        {
            DatabaseChecker.EnsureDatabaseExists();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Произошла ошибка при подключении к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
        }
    }
}
