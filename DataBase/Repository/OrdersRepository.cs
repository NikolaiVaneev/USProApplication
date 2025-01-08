using AutoMapper;
using Microsoft.EntityFrameworkCore;
using USProApplication.Models;
using USProApplication.Models.Repositories;

namespace USProApplication.DataBase.Repository
{
    public class OrdersRepository(IDbContextFactory<AppDbContext> _contextFactory, IMapper mapper) : IOrdersRepository
    {
        public async Task AddAsync(OrderDTO entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            var obj = mapper.Map<Entities.Order>(entity);

            if (entity.SelectedServicesIds != null)
            {
                foreach (var item in entity.SelectedServicesIds)
                {
                    var service = await context.Services.FindAsync(item);
                    if (service != null)
                    {
                        obj.Services.Add(service);
                    }
                }
            }

            await context.Orders.AddAsync(obj);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entity = await context.Orders.FindAsync(id);
            if (entity != null)
            {
                context.Orders.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public Task<List<OrderDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDTO?> GetByIdAsync(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entity = await context.Orders
                .Include(x => x.Services)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return mapper.Map<OrderDTO>(entity);
        }

        public async Task<ICollection<OrderShortInfo>> GetOrdersShortInfos()
        {
            await using var context = _contextFactory.CreateDbContext();

            var orders = await context.Orders.ToListAsync();
            return mapper.Map<List<OrderShortInfo>>(orders);
        }

        public Task UpdateAsync(OrderDTO entity)
        {
            throw new NotImplementedException();
        }
    }
}
