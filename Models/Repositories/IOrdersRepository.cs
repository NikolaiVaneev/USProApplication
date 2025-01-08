namespace USProApplication.Models.Repositories
{
    public interface IOrdersRepository : IBaseRepository<OrderDTO>
    {
        Task<ICollection<OrderShortInfo>> GetOrdersShortInfos();
    }
}