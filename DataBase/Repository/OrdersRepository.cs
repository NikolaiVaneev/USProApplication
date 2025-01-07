using AutoMapper;
using Microsoft.EntityFrameworkCore;
using USProApplication.Models;

namespace USProApplication.DataBase.Repository
{
    public class OrdersRepository(IDbContextFactory<AppDbContext> _contextFactory, IMapper mapper) : IOrdersRepository
    {
        public Task AddAsync(OrderDTO entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrderDTO?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
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
