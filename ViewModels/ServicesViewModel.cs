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
    [Reactive] public bool IsLoading { get; set; }
    public ICollectionView FilteredServices { get; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly IBaseRepository<Service> _repo;

    public ServicesViewModel(IBaseRepository<Service> repository)
    {
        _repo = repository;
        // Инициализация коллекции услуг из базы данных
        LoadServicesAsync();

        var filterApplier = this.WhenAnyValue(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => FilterServices());

        FilteredServices = CollectionViewSource.GetDefaultView(Services);
        FilteredServices.Filter = ServiceFilter;

        AddCommand = new AsyncCommand(AddServiceAsync);
        EditCommand = new AsyncCommand(EditServiceAsync, CanEditOrDelete);
        DeleteCommand = new AsyncCommand(DeleteServiceAsync, CanEditOrDelete);
    }

    private void FilterServices() => Application.Current.Dispatcher.Invoke(() => FilteredServices.Refresh());

    private async void LoadServicesAsync()
    {
        IsLoading = true;
        try
        {
            var services = await _repo.GetAllAsync();
            Services = new ObservableCollection<Service>(services);
        }
        finally
        {
            IsLoading = false;
        }
        
    }

    private Task RefreshServicesAsync()
    {
        // Вызываем LoadServicesAsync как Task
        LoadServicesAsync();
        return Task.CompletedTask;
    }

    private bool ServiceFilter(object obj)
    {
        if (obj is not Service service) return false;

        if (string.IsNullOrWhiteSpace(Filter)) return true;

        return service.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            || service.Abbreviation.Contains(Filter, StringComparison.OrdinalIgnoreCase)
            || (service.Description?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private async Task AddServiceAsync()
    {
        ServiceDialog dialog = new();

        if (!dialog.ShowDialog(new Service(), out Service? result))
            return;

        if (result != null)
        {
            result.Id = Guid.NewGuid();
            await _repo.AddAsync(result);

            await RefreshServicesAsync();
        }
    }

    private async Task EditServiceAsync()
    {
        if (SelectedService != null)
        {
            ServiceDialog dialog = new();

            if (!dialog.ShowDialog(SelectedService.Clone(), out Service? result))
                return;

            if (result != null)
            {
                await _repo.UpdateAsync(result);
                await RefreshServicesAsync();
            }
        }
    }

    private async Task DeleteServiceAsync()
    {
        if (SelectedService != null)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить выбранную услугу?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _repo.DeleteAsync(SelectedService.Id!.Value); 
                await RefreshServicesAsync();                      
            }
        }
    }

    private bool CanEditOrDelete() => SelectedService != null;
}
