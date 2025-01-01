namespace USProApplication.Models
{
    public interface ICounterpartyRepository : IBaseRepository<CounterpartyDTO>
    {
        Task<ICollection<ClientShortInfo>> GetCounterpartiesShortInfos();
    }
}