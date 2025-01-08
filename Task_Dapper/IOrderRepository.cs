using System.Collections.Generic;
using System.Threading.Tasks;

namespace Task_Dapper
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByFilterAsync(int? year, int? month, string status, int? productId);
        Task DeleteOrdersByFilterAsync(int? year, int? month, string status, int? productId);
    }
}