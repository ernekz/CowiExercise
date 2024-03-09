using Cowi.Models;
using System.Data.SqlClient;
using System.Transactions;

namespace Cowi.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(int productId, Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task ReduceProductQuantityAsync(SqlConnection connection, SqlTransaction transaction, int quantity, int productId);

        Task LockProductStockAsync(SqlConnection connection, SqlTransaction transaction, int productId);

    }
}
