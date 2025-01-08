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

    public event Action<OrderDTO?>? OnSave;

    private DelegateCommand? apply;
    public DelegateCommand Apply => apply ??= new DelegateCommand(() =>
    {
        Order.SelectedServicesIds = Services?
            .Where(s => s.IsChecked)
            .Select(s => s.Id)
            .ToList();
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

    private void UpdateSelectedServicesCount()
    {
        SelectedServicesCount = Services?.Count(s => s.IsChecked) ?? 0;
    }
}