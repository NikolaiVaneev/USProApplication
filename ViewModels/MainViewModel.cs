using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Controls;
using USProApplication.Views;

namespace USProApplication.ViewModels;

public class MainViewModel : ReactiveObject
{
    [Reactive] public Page? CurrentView { get; private set; }
    [Reactive] public string? CurrentScreenTitle { get; private set; }
    [Reactive] public PackIconKind? ScreenIcon { get; private set; }
    [Reactive] public MenuItemViewModel? SelectedMenuItem { get; set; }
    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    

    public MainViewModel()
    {
        MenuItems =
        [
            new() { Title = "Заказы", View = new OrdersPage(), Icon = PackIconKind.ShoppingCart },
            new() { Title = "Контрагенты", View = new CustomersPage(), Icon = PackIconKind.Account },
            new() { Title = "Услуги", View = new ServicesPage(), Icon = PackIconKind.Toolbox }
        ];

        // Установить начальный выбранный элемент, если он есть
        if (MenuItems.Any())
            SelectedMenuItem = MenuItems.First();

        // Реакция на изменения SelectedMenuItem
        this.WhenAnyValue(x => x.SelectedMenuItem)
            .Where(item => item != null) 
            .Subscribe(item =>
            {
                CurrentView = item!.View;
                CurrentScreenTitle = item!.Title;
                ScreenIcon = item!.Icon;
            });
    }
}

public class MenuItemViewModel
{
    public string Title { get; set; } = string.Empty;
    public Page View { get; set; } = new();
    public PackIconKind Icon { get; set; }
}