using Cowi.Models;
using System.Data.SqlClient;

namespace Cowi.Repositories
{
    public class ProductRepository : IProductRepository
    {

        private readonly IDbConnector _connector;

        public ProductRepository(IDbConnector dbConnector)
        {
            _connector = dbConnector;
        }
    
        public async Task<Product> CreateProductAsync(Product product)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = @"Insert into Products (Name, Description, Price, StockQuantity) 
                    Values (@Name, @Description, @Price, @StockQuantity); 
                    Select Scope_Identity();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

                    var newProductId = await command.ExecuteScalarAsync();

                    product.Id = Convert.ToInt32(newProductId);
                }
            }
            return product;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Delete from Products Where id = @id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", productId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();

            using (var connection = await _connector.OpenConnectionAsync())
            {

                var query = "Select * from Products";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(MapProductFromDR(reader));
                        }
                    }
                }
            }
            return null;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            using (var connection = await _connector.OpenConnectionAsync())
            {
                var query = "Select * from Products Where id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", productId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapProductFromDR(reader);
                        }
                    }
                }
            }
            return null;
        }


        public async Task ReduceProductQuantityAsync(SqlConnection connection, SqlTransaction transaction, int quantity, int productId)
        {
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;

                    command.CommandText = @"
                        Update Products
                        Set StockQuantity = StockQuantity - @Quantity
                        Where Id = @ProductId";

                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    await command.ExecuteNonQueryAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error reducing product quantity", ex);
            }
        }

        public async Task<Product> UpdateProductAsync(int productId, Product product)
        {
        
            using (var connection = await _connector.OpenConnectionAsync())
            {
           
                
                var query = @"
                    Update Products
                    Set Name = @Name,
                        Description = @Description,
                        Price = @Price,
                        StockQuantity = @StockQuantity
                    Where id = @id";

                using (var command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                    command.Parameters.AddWithValue("@id", productId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();

                    if(rowsAffected == 0) 
                    {
                        throw new InvalidOperationException("Update failed due to concurrency conflict");
                    }

                }
            }
            return product;
            
        }
        public async Task LockProductStockAsync(SqlConnection connection, SqlTransaction transaction, int productId)
        {
            var query = @"
                Select StockQuantity
                From Products WITH (UPDLOCK)
                Where Id = @ProductId";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ProductId", productId);

                await command.ExecuteNonQueryAsync();
            }
        }


        private Product MapProductFromDR(SqlDataReader reader)
        {
            return new Product
            {
                Id = (int)reader["id"],
                Name = (string)reader["Name"],
                Description = (string)reader["Description"],
                Price = (decimal)reader["Price"],
                StockQuantity = (int)reader["StockQuantity"]
            };
        }
    }
}
