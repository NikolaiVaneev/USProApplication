using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using USProApplication.ViewModels;

namespace USProApplication.Views
{
    public partial class ServicesPage : Page
    {
        public ServicesPage()
        {
            InitializeComponent();

            // Получаем ViewModel из DI
            DataContext = App.ServiceProvider?.GetService<ServicesViewModel>()
                          ?? throw new InvalidOperationException("Не удалось получить экземпляр ServicesViewModel.");
        }
    }
}
