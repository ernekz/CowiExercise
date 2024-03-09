using Cowi.Models;
using System.Data.SqlClient;

namespace Cowi.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> AddProductToOrderAsync(int orderId, OrderItem item);

        Task<Order> GetOpenOrderAsync(string userId);
        Task<Order> UpdateOrderAsync(Order order);

        Task<Order> RemoveProductFromOrderAsync(int orderId, int productId);

        Task<Order> UpdateProductQuantityInOrderAsync(int orderId, int productId, int newQuantity);

        Task<Order> GetOrderAsync(int orderId);

        Task<Order> DeleteOrderAsync(int orderId);

        Task<Order> CompleteOrderAsync(int orderId);
    }
}
