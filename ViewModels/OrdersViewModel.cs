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

public class OrdersViewModel : ReactiveObject
{
    [Reactive] public ObservableCollection<OrderShortInfo> Orders { get; set; } = [];
    [Reactive] public ObservableCollection<OrderShortInfo> FilteredOrders { get; set; } = [];
    [Reactive] public OrderShortInfo? SelectedOrder { get; set; }
    [Reactive] public string Filter { get; set; } = string.Empty;
    [Reactive] public bool IsLoading { get; set; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly IOrdersRepository _repo;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IMapper _mapper;

    public OrdersViewModel(IOrdersRepository repository, IDirectoryRepository directoryRepository, IMapper mapper)
    {
        _repo = repository;
        _directoryRepository = directoryRepository;
        _mapper = mapper;

        LoadOrdersAsync();

        // Подписка на изменения фильтра
        this.WhenAnyValue(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => ApplyFilter());

        AddCommand = new AsyncCommand(AddOrderAsync);
        EditCommand = new AsyncCommand(EditOrderAsync, CanEditOrDelete);
        DeleteCommand = new AsyncCommand(DeleteOrderAsync, CanEditOrDelete);
    }

    private async void LoadOrdersAsync()
    {
        IsLoading = true;
        try
        {
            var orders = await _repo.GetOrdersShortInfos();
            Orders = new ObservableCollection<OrderShortInfo>(orders);

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
        // Применяем фильтр к полной коллекции и обновляем FilteredOrders
        var filtered = string.IsNullOrWhiteSpace(Filter)
            ? Orders
            : new ObservableCollection<OrderShortInfo>(
                Orders.Where(orders =>
                    orders.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                    orders.ContractNo.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                    (orders.Address?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false))
              );

        FilteredOrders = filtered;
    }

    private async Task AddOrderAsync()
    {
        var executors = await _directoryRepository.GetCounterpartiesAsync(true);
        var clients = await _directoryRepository.GetCounterpartiesAsync(false);
        var services = await _directoryRepository.GetServicesAsync();
        OrderDialog dialog = new();

        if (dialog.ShowDialog(new OrderDTO(), executors, clients, services, out OrderDTO? result))
        {
            if (result != null)
            {
                result.Id = Guid.NewGuid();

                await _repo.AddAsync(result);
                Orders.Add(_mapper.Map<OrderShortInfo>(result));
                ApplyFilter();
            }
        }
    }

    private async Task EditOrderAsync()
    {
        if (SelectedOrder != null)
        {
            var executors = await _directoryRepository.GetCounterpartiesAsync(true);
            var clients = await _directoryRepository.GetCounterpartiesAsync(false);
            var services = await _directoryRepository.GetServicesAsync();
            OrderDialog dialog = new();

            var order = await _repo.GetByIdAsync(SelectedOrder.Id);

            if (order != null && dialog.ShowDialog(order, executors, clients, services, out OrderDTO? result))
            {

                if (result != null)
                {
                    await _repo.UpdateAsync(result);

                    // Обновляем запись в полной коллекции и фильтруем
                    var index = Orders.IndexOf(SelectedOrder);
                    if (index >= 0)
                    {
                        Orders[index] = _mapper.Map<OrderShortInfo>(result);
                    }

                    ApplyFilter();
                }

            }
        }
    }

    private async Task DeleteOrderAsync()
    {
        if (SelectedOrder != null)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить выбранный заказ?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _repo.DeleteAsync(SelectedOrder.Id);

                // Удаляем из полной коллекции и фильтруем
                Orders.Remove(SelectedOrder);
                ApplyFilter();
            }
        }
    }

    private bool CanEditOrDelete() => SelectedOrder != null;
}
