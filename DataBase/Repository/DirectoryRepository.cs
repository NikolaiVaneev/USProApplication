using Microsoft.EntityFrameworkCore;
using USProApplication.Models;
using USProApplication.Models.Repositories;

namespace USProApplication.DataBase.Repository
{
    public class DirectoryRepository(IDbContextFactory<AppDbContext> _contextFactory) : IDirectoryRepository
    {
        public async Task<ICollection<DictionaryItem>> GetCounterpartiesAsync(bool isExecutor)
        {
            await using var context = _contextFactory.CreateDbContext();

            if (isExecutor)
            {
                return await context.Counterparties.Where(x => x.Executor == true)
                    .OrderBy(x => x.Name)
                    .Select(x => new DictionaryItem { Id = x.Id, Name = $"{x.Name} {x.Bank}" })
                    .ToListAsync();
            }
            else
            {
                return await context.Counterparties.Where(x => x.Executor == false)
                    .Select(x => new DictionaryItem { Id = x.Id, Name = x.Name })
                    .OrderBy(x => x.Name)
                    .ToListAsync();
            }
        }

        public async Task<ICollection<ServiceItem>> GetServicesAsync()
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Services
                .Select(x => new ServiceItem { Id = x.Id, Name = x.Name, Price = x.Price })
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
