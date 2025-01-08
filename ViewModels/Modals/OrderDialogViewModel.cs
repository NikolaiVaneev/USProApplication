using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using USProApplication.Models;
using USProApplication.Utils;

namespace USProApplication.ViewModels.Modals;

public class OrderDialogViewModel : ReactiveObject
{
    [Reactive] public OrderDTO? Order { get; set; }
    [Reactive] public ICollection<DictionaryItem>? Executors { get; set; }
    [Reactive] public ICollection<DictionaryItem>? Customers { get; set; }
    [Reactive] public ICollection<ServiceItem>? Services { get; set; } = [];

    public event Action<OrderDTO?>? OnSave;

    private DelegateCommand? apply;
    public DelegateCommand Apply => apply ??= new DelegateCommand(() =>
    {
        Order.SelectedServicesIds = Services?.Where(s => s.IsChecked).Select(s => s.Id).ToList();
        OnSave?.Invoke(Order);
    }, () => !string.IsNullOrWhiteSpace(Order?.Number));
}
