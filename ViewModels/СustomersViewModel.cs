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

public class CustomersViewModel : ReactiveObject
{
    [Reactive] public ObservableCollection<ClientShortInfo>? Clients { get; set; } = [];
    [Reactive] public ObservableCollection<ClientShortInfo>? FilteredClients { get; set; } = [];
    [Reactive] public ClientShortInfo? SelectedClient { get; set; }
    [Reactive] public string Filter { get; set; } = string.Empty;
    [Reactive] public bool IsLoading { get; set; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly ICounterpartyRepository _repo;

    public CustomersViewModel(ICounterpartyRepository repository)
    {
        _repo = repository;

        LoadClientsAsync();

        var filterApplier = this.WhenAnyValue(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => FilterClients());

        AddCommand = new DelegateCommand(AddCounterparty);
        EditCommand = new DelegateCommand(EditCounterparty, CanEditOrDelete);
        DeleteCommand = new DelegateCommand(DeleteCounterparty, CanEditOrDelete);
    }

    private async void LoadClientsAsync()
    {
        IsLoading = true;
        try
        {
            var clients = await _repo.GetCounterpartiesShortInfos();
            Clients = new ObservableCollection<ClientShortInfo>(clients);

            // Применяем фильтр после загрузки данных
            //ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void FilterClients() => Application.Current.Dispatcher.Invoke(() => {
    
    });

    private bool ClientsFilter(object obj)
    {
        if (obj is not ClientShortInfo service) return false;

        if (string.IsNullOrWhiteSpace(Filter)) return true;

        return service.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            || service.Address.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            || (service.ChiefFullName?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private void AddCounterparty()
    {
        СounterpartyDialog dialog = new();

        if (!dialog.ShowDialog(new CounterpartyDTO(), out CounterpartyDTO? result))
            return;

        if (result != null)
        {
            result.Id = Guid.NewGuid();
            //await _repo.AddAsync(result);

            //// Обновляем полную коллекцию и фильтруем
            //Services.Add(result);
            //ApplyFilter();
        }
    }

    private void EditCounterparty()
    {
        //if (SelectedService != null)
        //{
        //    ServiceDialog dialog = new();

        //    if (!dialog.ShowDialog(SelectedService.Clone(), out Service? result))
        //        return;

        //    if (result != null)
        //    {
        //        var currentService = Services?.First(x => x.Id == result.Id);
        //        currentService = result;
        //    }
        //}
    }

    private void DeleteCounterparty()
    {
        if (SelectedClient != null)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить выбранного контрагента?\nВсе связанные с ним заказы будут удалены.", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Clients?.Remove(SelectedClient);
            }

        }
    }

    private bool CanEditOrDelete() => SelectedClient != null;
}
