using AutoMapper;
using Microsoft.EntityFrameworkCore;
using USProApplication.DataBase.Entities;
using USProApplication.Models;

namespace USProApplication.DataBase.Repository;

public class CounterpartyRepository(IDbContextFactory<AppDbContext> _contextFactory, IMapper mapper) : ICounterpartyRepository
{
    public async Task AddAsync(CounterpartyDTO counterparty)
    {
        await using var context = _contextFactory.CreateDbContext();
    
        var entity = mapper.Map<Counterparty>(counterparty);
        await context.Counterparties.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = _contextFactory.CreateDbContext();
        var entity = await context.Counterparties.FindAsync(id);
        if (entity != null)
        {
            context.Counterparties.Remove(entity);
            await context.SaveChangesAsync();
        }
    }

    public Task<List<CounterpartyDTO>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<CounterpartyDTO?> GetByIdAsync(Guid id)
    {
        await using var context = _contextFactory.CreateDbContext();
        var entity = await context.Counterparties.FindAsync(id);
        return mapper.Map<CounterpartyDTO>(entity);
    }

    public async Task<ICollection<ClientShortInfo>> GetCounterpartiesShortInfos()
    {
        await using var context = _contextFactory.CreateDbContext();

        var counterparties = await context.Counterparties.ToListAsync();
        return mapper.Map<List<ClientShortInfo>>(counterparties);
    }

    public async Task UpdateAsync(CounterpartyDTO counterparty)
    {
        await using var context = _contextFactory.CreateDbContext();

        var entity = mapper.Map<Counterparty>(counterparty);
        context.Counterparties.Update(entity);
        await context.SaveChangesAsync();
    }
}
