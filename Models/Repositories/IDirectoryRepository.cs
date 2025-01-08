namespace USProApplication.Models.Repositories;

public interface IDirectoryRepository
{
    Task<ICollection<DictionaryItem>> GetCounterpartiesAsync(bool isExecutor);
    Task<ICollection<ServiceItem>> GetServicesAsync();
}
