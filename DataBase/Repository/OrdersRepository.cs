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

            obj.ParentOrder = null;

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
                .Include(x => x.ParentOrder)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return mapper.Map<OrderDTO>(entity);
        }

        public async Task<ICollection<OrderShortInfo>> GetOrdersShortInfos()
        {
            await using var context = _contextFactory.CreateDbContext();

            var orders = await context.Orders.OrderByDescending(x => x.StartDate).ToListAsync();
            return mapper.Map<List<OrderShortInfo>>(orders);
        }

        public async Task UpdateAsync(OrderDTO order)
        {
            await using var context = _contextFactory.CreateDbContext();

            // Получить существующий заказ с его услугами из базы данных
            var existingOrder = await context.Orders
                .Include(o => o.Services) // Загрузка связанных данных
                .FirstOrDefaultAsync(o => o.Id == order.Id) ?? throw new InvalidOperationException($"Order with ID {order.Id} not found.");

            // Удаление существующих связей
            existingOrder.Services.Clear();

            // Добавление новых связей
            if (order.SelectedServicesIds != null)
            {
                foreach (var serviceId in order.SelectedServicesIds)
                {
                    var service = await context.Services.FindAsync(serviceId);
                    if (service != null)
                    {
                        existingOrder.Services.Add(service);
                    }
                }
            }

            // Обновление прочих данных заказа
            mapper.Map(order, existingOrder);

            // Сохранение изменений
            await context.SaveChangesAsync();
        }
    }
}
