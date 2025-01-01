using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using USProApplication.ViewModels;

namespace USProApplication.Views
{
    public partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            InitializeComponent();

            // Получаем ViewModel из DI
            DataContext = App.ServiceProvider?.GetService<CustomersViewModel>()
                          ?? throw new InvalidOperationException("Не удалось получить экземпляр CustomersViewModel.");
        }
    }
}
