using Cowi.Helper;
using Cowi.Models;
using Cowi.Repositories;
using Microsoft.AspNetCore.Http;

namespace Cowi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IDbConnector _dbConnector;
        public OrderService(IDbConnector connector, IJwtHelper jwtHelper, IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _jwtHelper = jwtHelper;
            _dbConnector = connector;
        }

        public async Task<Order> AddProductToOrderAsync(int productId, int quantity, string jwtToken)
        {
            
            var userId = _jwtHelper.ValidateJwtToken(jwtToken);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            Order order = await _orderRepository.GetOpenOrderAsync(userId);

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    Items = new List<OrderItem>()

                };

                order = await _orderRepository.CreateOrderAsync(order);
            }

            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found", nameof(productId));

            }

            
            var existingItem = order.Items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                

                var newQuantity = existingItem.Quantity + quantity;

                if(newQuantity > product.StockQuantity) 
                {
                    throw new InvalidOperationException("Insufficient stock quantity");
                }

                await _orderRepository.UpdateProductQuantityInOrderAsync(order.Id, product.Id, newQuantity);

                order = await _orderRepository.GetOpenOrderAsync(userId);
                order.TotalPrice = order.Items.Sum(item => item.Quantity * item.Price);
            }
            else
            {
                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                order.Items.Add(orderItem);

                await _orderRepository.AddProductToOrderAsync(order.Id, orderItem);
                order.TotalPrice = order.Items.Sum(item => item.Quantity * item.Price);
            }

            if (order.Id == 0)
            {
                order = await _orderRepository.CreateOrderAsync(order);
            }
            else
            {
                order = await _orderRepository.UpdateOrderAsync(order);
            }

            return order;
        }

        public async Task<Order> CompleteOrderAsync(string jwtToken)
        {
            var userId = _jwtHelper.ValidateJwtToken(jwtToken);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            

            using (var connection = await _dbConnector.OpenConnectionAsync())
            {
                var transaction = connection.BeginTransaction();


                try
                {
                    var order = await _orderRepository.GetOpenOrderAsync(userId);
                    if (order == null)
                    {
                        throw new ArgumentException("User does not have any open orders");
                    }
                    foreach (var item in order.Items)
                    {
                        await _productRepository.LockProductStockAsync(connection, transaction, item.ProductId);

                        await _productRepository.ReduceProductQuantityAsync(connection, transaction, item.Quantity, item.ProductId);

                    }
                    await _orderRepository.CompleteOrderAsync(order.Id);

                    transaction.Commit();

                    return order;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<Order> DeleteOrderAsync(string jwtToken)
        {

            var userId = _jwtHelper.ValidateJwtToken(jwtToken);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var order = await _orderRepository.GetOpenOrderAsync(userId);

            if (order == null)
            {
                throw new ArgumentException("User does not have open orders");
            }


            return await _orderRepository.DeleteOrderAsync(order.Id);
        }

        public async Task<Order> GetCurrentUserOrderAsync(string jwtToken)
        {
            
            var userId = _jwtHelper.ValidateJwtToken(jwtToken);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            return await _orderRepository.GetOpenOrderAsync(userId);
        }

        public async Task<Order> RemoveProductFromOrderAsync(int productId, string jwtToken)
        {
            var userId = _jwtHelper.ValidateJwtToken(jwtToken);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var product = await _productRepository.GetProductByIdAsync(productId);

            if(product == null)
            {
                throw new ArgumentException("Product doesnt exist", nameof(productId));
            }
            Order order = await _orderRepository.GetOpenOrderAsync(userId);
            if (order == null)
            {
                throw new ArgumentException("Order not found", nameof(order.Id));
            }
            

            var removedProduct = order.Items.FirstOrDefault(item => item.ProductId == productId);
            if (removedProduct == null)
            {
                throw new ArgumentException("Product not found in the order", nameof(productId));
               
            }
            order.Items.Remove(removedProduct);
            await _orderRepository.RemoveProductFromOrderAsync(order.Id,productId);

            order.TotalPrice = order.Items.Sum(item => item.Quantity * item.Price);

            await _orderRepository.UpdateOrderAsync(order);


            return order;
        }

        public Task<Order> UpdateProductQuantityInOrderAsync(int orderId, int productId, int newQuantity, string jwtToken)
        {
            throw new NotImplementedException();
        }
    }
}
