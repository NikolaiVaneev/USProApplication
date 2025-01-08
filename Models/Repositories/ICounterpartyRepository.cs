namespace USProApplication.Models.Repositories
{
    public interface ICounterpartyRepository : IBaseRepository<CounterpartyDTO>
    {
        Task<ICollection<ClientShortInfo>> GetCounterpartiesShortInfos();
    }
}