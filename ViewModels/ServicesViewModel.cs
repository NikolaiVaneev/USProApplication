using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using USProApplication.Models;
using USProApplication.Utils;
using USProApplication.Views.Modals;

namespace USProApplication.ViewModels;

public class ServicesViewModel : ReactiveObject
{
    [Reactive] public ObservableCollection<Service>? Services { get; set; }
    [Reactive] public Service? SelectedService { get; set; }
    [Reactive] public string Filter { get; set; } = string.Empty;

    public ICollectionView FilteredServices { get; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public ServicesViewModel()
    {
        var filterApplier = this.WhenAnyValue(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => FilterServices());

        Services =
        [
            new Service { Id = Guid.NewGuid(), Name = "Система контроля", Abbreviation = "СКУД", Price = 500 },
            new Service { Id = Guid.NewGuid(), Name = "Пожарная сигнализация", Abbreviation = "АПС", Price = 1000, Description = "Описание услуги 2" },
            new Service { Id = Guid.NewGuid(), Name = "ОВиК", Abbreviation = "ОВиК", Price = 1500, Description = "Описание услуги 3" }
        ];

        FilteredServices = CollectionViewSource.GetDefaultView(Services);
        FilteredServices.Filter = ServiceFilter;

        AddCommand = new DelegateCommand(AddService);
        EditCommand = new DelegateCommand(EditService, CanEditOrDelete);
        DeleteCommand = new DelegateCommand(DeleteService, CanEditOrDelete);
    }

    private void FilterServices() => Application.Current.Dispatcher.Invoke(() => FilteredServices.Refresh());

    private bool ServiceFilter(object obj)
    {
        if (obj is not Service service) return false;

        if (string.IsNullOrWhiteSpace(Filter)) return true;

        return service.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            || service.Abbreviation.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            || (service.Description?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private void AddService()
    {
        ServiceDialog dialog = new();
        
        if (!dialog.ShowDialog(new Service(), out Service? result))
            return;

        if (result != null)
        {
            result.Id = Guid.NewGuid();
            Services?.Add(result);
        }
    }

    private void EditService()
    {
        if (SelectedService != null)
        {
            ServiceDialog dialog = new();

            if (!dialog.ShowDialog(SelectedService.Clone(), out Service? result))
                return;

            if (result != null)
            {
               var currentService =  Services?.First(x => x.Id == result.Id);
               currentService = result;
            }
        }
    }

    private void DeleteService()
    {
        if (SelectedService != null)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить выбранную услугу?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Services?.Remove(SelectedService);
            }

        }
    }

    private bool CanEditOrDelete() => SelectedService != null;
}
