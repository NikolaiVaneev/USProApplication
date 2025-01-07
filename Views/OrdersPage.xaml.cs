using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using USProApplication.ViewModels;

namespace USProApplication.Views
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();

            // Получаем ViewModel из DI
            DataContext = App.ServiceProvider?.GetService<OrdersViewModel>()
                          ?? throw new InvalidOperationException("Не удалось получить экземпляр OrdersViewModel.");
        }
    }
}
