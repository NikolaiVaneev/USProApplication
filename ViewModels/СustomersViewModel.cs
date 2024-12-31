using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using USProApplication.DataBase.Entities;
using USProApplication.Models;
using USProApplication.Utils;
using USProApplication.Views.Modals;

namespace USProApplication.ViewModels;

public class CustomersViewModel : ReactiveObject
{
    [Reactive] public ObservableCollection<ClientShortInfo>? Clients { get; set; }
    [Reactive] public ClientShortInfo? SelectedClient { get; set; }
    [Reactive] public string Filter { get; set; } = string.Empty;

    public ICollectionView FilteredClients { get; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public CustomersViewModel()
    {
        var filterApplier = this.WhenAnyValue(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => FilterClients());

        Clients =
        [
            new ClientShortInfo { Id = Guid.NewGuid(), Name = "ООО Компания 1", ChiefFullName = "Иванов Иван Иванович", Address = "г. Москва, ул. Ленина, д. 1", CreatedOn = DateTime.Now, ContractDate = DateTime.Now},
            new ClientShortInfo { Id = Guid.NewGuid(), Name = "ООО Компания 2", ChiefFullName = "Петров Петр Петрович", Address = "г. Москва, ул. Ленина, д. 2", CreatedOn = DateTime.Now},
            new ClientShortInfo { Id = Guid.NewGuid(), Name = "ООО Компания 3", ChiefFullName = "Сидоров Сидор Сидорович", Address = "г. Москва, ул. Ленина, д. 3", CreatedOn = DateTime.Now, ContractDate = DateTime.Now},
            new ClientShortInfo { Id = Guid.NewGuid(), Name = "ООО Компания 4", ChiefFullName = "Федоров Федор Федорович", Address = "г. Москва, ул. Ленина, д. 4", CreatedOn = DateTime.Now},
            new ClientShortInfo { Id = Guid.NewGuid(), Name = "ООО Компания 5", ChiefFullName = "Михайлов Михаил Михайлович", Address = "г. Москва, ул. Ленина, д. 5", CreatedOn = DateTime.Now}
        ];

        FilteredClients = CollectionViewSource.GetDefaultView(Clients);
        FilteredClients.Filter = ClientsFilter;

        AddCommand = new DelegateCommand(AddCounterparty);
        EditCommand = new DelegateCommand(EditService, CanEditOrDelete);
        DeleteCommand = new DelegateCommand(DeleteService, CanEditOrDelete);
    }

    private void FilterClients() => Application.Current.Dispatcher.Invoke(() => FilteredClients.Refresh());

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

    private void EditService()
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

    private void DeleteService()
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
