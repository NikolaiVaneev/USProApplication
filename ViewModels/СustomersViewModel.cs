using AutoMapper;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using USProApplication.Models;
using USProApplication.Models.Repositories;
using USProApplication.Utils;
using USProApplication.Views.Modals;

namespace USProApplication.ViewModels;

public class CustomersViewModel : ReactiveObject
{
    [Reactive] public ObservableCollection<ClientShortInfo> Clients { get; set; } = [];
    [Reactive] public ObservableCollection<ClientShortInfo> FilteredClients { get; set; } = [];
    [Reactive] public ClientShortInfo? SelectedClient { get; set; }
    [Reactive] public string Filter { get; set; } = string.Empty;
    [Reactive] public bool IsLoading { get; set; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly ICounterpartyRepository _repo;
    private readonly IMapper _mapper;
    public CustomersViewModel(IMapper mapper, ICounterpartyRepository repository)
    {
        _mapper = mapper;
        _repo = repository;

        LoadClientsAsync();

        var filterApplier = this.WhenAnyValue(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => ApplyFilter());

        AddCommand = new AsyncCommand(AddCounterparty);
        EditCommand = new AsyncCommand(EditCounterparty, CanEditOrDelete);
        DeleteCommand = new AsyncCommand(DeleteCounterparty, CanEditOrDelete);
    }

    private async void LoadClientsAsync()
    {
        IsLoading = true;
        try
        {
            var clients = await _repo.GetCounterpartiesShortInfos();
            Clients = new ObservableCollection<ClientShortInfo>(clients);

            // Применяем фильтр после загрузки данных
            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        // Применяем фильтр к полной коллекции и обновляем FilteredServices
        var filtered = string.IsNullOrWhiteSpace(Filter)
            ? Clients
        : new ObservableCollection<ClientShortInfo>(
                Clients.Where(service =>
                    service.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                    service.Address.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                    (service.ChiefFullName?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false))
              );

        FilteredClients = filtered;
    }

    private async Task AddCounterparty()
    {
        СounterpartyDialog dialog = new();

        if (!dialog.ShowDialog(new CounterpartyDTO(), out CounterpartyDTO? result))
            return;

        if (result != null)
        {
            result.Id = Guid.NewGuid();
            result.CreatedOn = DateTime.Now;

            await _repo.AddAsync(result);

            Clients.Add(_mapper.Map<ClientShortInfo>(result));
            ApplyFilter();
        }
    }

    private async Task EditCounterparty()
    {
        if (SelectedClient != null)
        {
            СounterpartyDialog dialog = new();

            var counterparty = await _repo.GetByIdAsync(SelectedClient.Id);
            if (counterparty != null)
            {
                if (!dialog.ShowDialog(counterparty, out CounterpartyDTO? result))
                    return;

                if (result != null)
                {
                    await _repo.UpdateAsync(result);

                    // Обновляем запись в полной коллекции и фильтруем
                    var index = Clients.IndexOf(SelectedClient);
                    if (index >= 0)
                    {
                        Clients[index] = _mapper.Map<ClientShortInfo>(result);
                    }

                    ApplyFilter();
                }
            }
        }
    }

    private async Task DeleteCounterparty()
    {
        if (SelectedClient != null)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить выбранного контрагента?\nВсе связанные с ним заказы будут удалены.", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _repo.DeleteAsync(SelectedClient.Id);
                Clients?.Remove(SelectedClient);
                ApplyFilter();
            }

        }
    }

    private bool CanEditOrDelete() => SelectedClient != null;
}
