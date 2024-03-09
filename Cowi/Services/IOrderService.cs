using Cowi.Models;

namespace Cowi.Services
{
    public interface IOrderService
    {
        Task<Order> AddProductToOrderAsync(int productId, int quantity, string jwtToken);
        Task<Order> RemoveProductFromOrderAsync(int productId, string jwtToken);

        Task<Order> GetCurrentUserOrderAsync(string jwtToken);
        Task<Order> UpdateProductQuantityInOrderAsync(int orderId, int productId, int newQuantity, string jwtToken);

        Task<Order> CompleteOrderAsync(string jwtToken);

        Task<Order> DeleteOrderAsync(string jwtToken);
    }
}
