using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using USProApplication.Models;
using USProApplication.Utils;

namespace USProApplication.ViewModels.Modals;

public class OrderDialogViewModel : ReactiveObject
{
    [Reactive] public OrderDTO? Order { get; set; }
    [Reactive] public ICollection<DictionaryItem>? Executors { get; set; }
    [Reactive] public ICollection<DictionaryItem>? Customers { get; set; }
    [Reactive] public ObservableCollection<ServiceItem>? Services { get; set; } = [];
    [Reactive] public int SelectedServicesCount { get; set; }

    [Reactive] public decimal TotalPrice { get; set; } = 0.0m;
    [Reactive] public decimal PriceToMeter { get; set; } = 0.0m;

    public event Action<OrderDTO?>? OnSave;

    private DelegateCommand? apply;
    public DelegateCommand Apply => apply ??= new DelegateCommand(() =>
    {
        Order.SelectedServicesIds = Services?
            .Where(s => s.IsChecked)
            .Select(s => s.Id)
            .ToList();

        Order.PriceToMeter = PriceToMeter;
        Order.Price = TotalPrice;

        OnSave?.Invoke(Order);
    }, () => !string.IsNullOrWhiteSpace(Order?.Number));

    public void InitializeServices(ICollection<ServiceItem> services, ICollection<Guid>? selectedServiceIds)
    {
        Services = new ObservableCollection<ServiceItem>(services);

        if (selectedServiceIds != null)
        {
            foreach (var service in Services)
            {
                service.IsChecked = selectedServiceIds.Contains(service.Id);
            }
        }

        foreach (var service in Services)
        {
            service.WhenAnyValue(s => s.IsChecked)
                .Subscribe(_ => UpdateSelectedServicesCount());
        }

        UpdateSelectedServicesCount();
    }

    private DelegateCommand? calculatePrice;

    public DelegateCommand CalculatePrice => calculatePrice ??= new DelegateCommand(() =>
    {
        if (Order == null) return;

        int square = Order.Square;
        decimal priceToMeter = 0;
        decimal fullPrice = 0;


        var usingServices = Services?.Where(s => s.IsChecked).ToList();
        if (usingServices != null && usingServices.Count > 0)
        {
            foreach (var service in usingServices)
            {
                priceToMeter += service.Price;
            }

            fullPrice = priceToMeter * square;

            if (Order.UsingNDS && Order.NDS > 0)
            {
                priceToMeter = Math.Round(priceToMeter * (Order.NDS + 100) / 100, 2);
                fullPrice = Math.Round(fullPrice * (Order.NDS + 100) / 100, 2);
            }

            PriceToMeter = priceToMeter;
            TotalPrice = fullPrice;
        }

    });

    private void UpdateSelectedServicesCount()
    {
        SelectedServicesCount = Services?.Count(s => s.IsChecked) ?? 0;
    }
}