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

public class ServicesViewModel : ReactiveObject
{
    [Reactive] public ObservableCollection<Service> Services { get; set; } = [];
    [Reactive] public ObservableCollection<Service> FilteredServices { get; set; } = [];
    [Reactive] public Service? SelectedService { get; set; }
    [Reactive] public string Filter { get; set; } = string.Empty;
    [Reactive] public bool IsLoading { get; set; }

    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly IBaseRepository<Service> _repo;

    public ServicesViewModel(IBaseRepository<Service> repository)
    {
        _repo = repository;

        LoadServicesAsync();

        // Подписка на изменения фильтра
        this.WhenAnyValue(x => x.Filter)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => ApplyFilter());

        AddCommand = new AsyncCommand(AddServiceAsync);
        EditCommand = new AsyncCommand(EditServiceAsync, CanEditOrDelete);
        DeleteCommand = new AsyncCommand(DeleteServiceAsync, CanEditOrDelete);
    }

    private async void LoadServicesAsync()
    {
        IsLoading = true;
        try
        {
            var services = await _repo.GetAllAsync();
            Services = new ObservableCollection<Service>(services);

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
            ? Services
            : new ObservableCollection<Service>(
                Services.Where(service =>
                    service.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                    service.Abbreviation.Contains(Filter, StringComparison.OrdinalIgnoreCase) ||
                    (service.Description?.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false))
              );

        FilteredServices = filtered;
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

            // Обновляем полную коллекцию и фильтруем
            Services.Add(result);
            ApplyFilter();
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

                // Обновляем запись в полной коллекции и фильтруем
                var index = Services.IndexOf(SelectedService);
                if (index >= 0)
                {
                    Services[index] = result;
                }

                ApplyFilter();
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

                // Удаляем из полной коллекции и фильтруем
                Services.Remove(SelectedService);
                ApplyFilter();
            }
        }
    }

    private bool CanEditOrDelete() => SelectedService != null;
}
