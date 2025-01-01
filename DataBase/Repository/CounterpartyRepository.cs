using AutoMapper;
using USProApplication.Models;

namespace USProApplication.DataBase.Repository;

public class CounterpartyRepository(IMapper mapper) : ICounterpartyRepository
{
    public Task AddAsync(CounterpartyDTO entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<CounterpartyDTO>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CounterpartyDTO?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<ClientShortInfo>> GetCounterpartiesShortInfos()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CounterpartyDTO entity)
    {
        throw new NotImplementedException();
    }
}
