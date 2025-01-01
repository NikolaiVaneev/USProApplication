using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using USProApplication.Models;
using USProApplication.Utils;
using USProApplication.Views.Modals;

namespace USProApplication.ViewModels;

public class OrdersViewModel : ReactiveObject
{
    [Reactive] public ObservableCollection<ClientShortInfo> Сounterparties { get; set; } = [];
    [Reactive] public ObservableCollection<ClientShortInfo> FilteredСounterparties { get; set; } = [];
    [Reactive] public ClientShortInfo? SelectedCounterparty { get; set; }
    [Reactive] public string Filter { get; set; } = string.Empty;
    [Reactive] public bool IsLoading { get; set; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    //private readonly IBaseRepository<CounterpartyDTO> _repo;

    //public OrdersViewModel(IBaseRepository<CounterpartyDTO> repository)
    //{
    //    _repo = repository;

    //    LoadСounterpartiesAsync();

    //    // Подписка на изменения фильтра
    //    this.WhenAnyValue(x => x.Filter)
    //        .Throttle(TimeSpan.FromMilliseconds(300))
    //        .Subscribe(_ => ApplyFilter());

    //    AddCommand = new AsyncCommand(AddCounterpartyAsync);
    //    EditCommand = new AsyncCommand(EditCounterpartyAsync, CanEditOrDelete);
    //    DeleteCommand = new AsyncCommand(DeleteCounterpartyAsync, CanEditOrDelete);
    //}

    //private async void LoadСounterpartiesAsync()
    //{
    //    IsLoading = true;
    //    try
    //    {
    //        var counterparties = await _repo.GetAllAsync();
    //        Сounterparties = new ObservableCollection<ClientShortInfo>(counterparties);

    //        // Применяем фильтр после загрузки данных
    //        ApplyFilter();
    //    }
    //    finally
    //    {
    //        IsLoading = false;
    //    }
    //}

    //private void ApplyFilter()
    //{
    //    // Применяем фильтр к полной коллекции и обновляем FilteredServices
    //    var filtered = string.IsNullOrWhiteSpace(Filter)
    //        ? Сounterparties
    //        : new ObservableCollection<Service>(
    //            Сounterparties.Where(service =>
    //                service.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
    //                service.ChiefFullName?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
    //                (service.Address?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false))
    //          );

    //    FilteredСounterparties = filtered;
    //}

    //private async Task AddCounterpartyAsync()
    //{
    //    СounterpartyDialog dialog = new();

    //    if (!dialog.ShowDialog(new CounterpartyDTO(), out CounterpartyDTO? result))
    //        return;

    //    if (result != null)
    //    {
    //        result.Id = Guid.NewGuid();
    //        await _repo.AddAsync(result);

    //        // Обновляем полную коллекцию и фильтруем
    //        Сounterparties.Add(result);
    //        ApplyFilter();
    //    }
    //}

    //private async Task EditCounterpartyAsync()
    //{
    //    if (SelectedCounterparty != null)
    //    {
    //        СounterpartyDialog dialog = new();

    //        if (!dialog.ShowDialog(SelectedService.Clone(), out CounterpartyDTO? result))
    //            return;

    //        if (result != null)
    //        {
    //            await _repo.UpdateAsync(result);

    //            // Обновляем запись в полной коллекции и фильтруем
    //            //var index = Services.IndexOf(SelectedService);
    //            //if (index >= 0)
    //            //{
    //            //    Services[index] = result;
    //            //}

    //            ApplyFilter();
    //        }
    //    }
    //}

    //private async Task DeleteCounterpartyAsync()
    //{
    //    if (SelectedCounterparty != null)
    //    {
    //        var result = MessageBox.Show("Вы действительно хотите удалить выбранного контрагента?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
    //        if (result == MessageBoxResult.Yes)
    //        {
    //            await _repo.DeleteAsync(SelectedCounterparty.Id);

    //            // Удаляем из полной коллекции и фильтруем
    //            Сounterparties.Remove(SelectedCounterparty);
    //            ApplyFilter();
    //        }
    //    }
    //}

    private bool CanEditOrDelete() => SelectedCounterparty != null;
}
