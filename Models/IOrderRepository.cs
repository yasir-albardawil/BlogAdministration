using static NuGet.Packaging.PackagingConstants;

namespace PieShop.Models
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task<Order?> FindAsync(int? id);
    }
}
