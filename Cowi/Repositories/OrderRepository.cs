using Cowi.Models;
using System.Data.SqlClient;

namespace Cowi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnector _connector;

        public OrderRepository(IDbConnector dbConnector)
        {
            _connector = dbConnector;
        }

        public async Task<Order> AddProductToOrderAsync(int orderId, OrderItem item)
        {
            
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price) VALUES (@OrderId, @ProductId, @Quantity, @Price);";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@ProductId", item.ProductId);
                    command.Parameters.AddWithValue("@Quantity", item.Quantity);
                    command.Parameters.AddWithValue("@Price", item.Price);
                    await command.ExecuteNonQueryAsync();
                    
                }
            }
            return await GetOrderAsync(orderId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Insert into Orders (TotalPrice, UserId, StatusId) Values (@TotalPrice, @UserId, @StatusId); Select Scope_Identity();";
           
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                    command.Parameters.AddWithValue("@UserId", order.UserId);
                    command.Parameters.AddWithValue("@StatusId", 1);
                    int orderId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    order.Id = orderId;
                    
                }
                    
            }
            return order;
        }

        public async Task<Order> GetOpenOrderAsync(string userId)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = @"
                    Select O.*
                    From Orders O
                    Inner Join OrderStatus OS ON O.StatusId = OS.Id
                    Where O.UserId = @UserId AND OS.Name = 'Open';
                    ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Order order = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                                UserId = reader.GetString(reader.GetOrdinal("UserId"))
                            };
                            order.Items = await GetOrderItemsAsync(order.Id);
                            return order;
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Select * From OrderItems Where OrderId = @OrderId";

                using(var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                OrderItemId = reader.GetInt32(reader.GetOrdinal("OrderItemId"))
                            };
                            orderItems.Add(orderItem);
                        }
                    }
                }
            }
            return orderItems;
        }

        public async Task<Order> RemoveProductFromOrderAsync(int orderId, int productId)
        {
            
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = @"
                Delete from OrderItems
                Where OrderId = @OrderId
                And ProductId = @ProductId
                ";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return null;

        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = @"
                    Update Orders
                    Set TotalPrice = @TotalPrice
                    Where OrderId = @OrderId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                    command.Parameters.AddWithValue("@OrderId", order.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return order;
        }

        public async Task<Order> UpdateProductQuantityInOrderAsync(int orderId, int productId, int newQuantity)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = @"
                Update OrderItems
                Set Quantity = @NewQuantity
                Where OrderId = @OrderId
                And ProductId = @ProductId;
                ";

                using (var command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@NewQuantity", newQuantity);
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return null;
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
      
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = @"
                    SELECT OrderId, TotalPrice, UserId 
                    FROM Orders 
                    WHERE OrderId = @OrderId;
                    ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Order order = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                                UserId = reader.GetString(reader.GetOrdinal("UserId")),
                            };
                            return order;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async Task<Order> DeleteOrderAsync(int orderId)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = @"
                    Delete from Orders
                    Where OrderId = @OrderId;
                    Delete from OrderItems
                    Where OrderId = @OrderId;
                ";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return null;
        }

        public async Task<Order> CompleteOrderAsync(int orderId)
        {
            using (var connection = await _connector.OpenConnectionAsync()) 
            {
                var query = @"
                    Update Orders
                    Set StatusId = 2
                    Where OrderId = @OrderId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return null;
        }

        
    }
}
