namespace USProApplication.Models
{
    public interface IOrdersRepository : IBaseRepository<OrderDTO>
    {
        Task<ICollection<OrderShortInfo>> GetOrdersShortInfos();
    }
}