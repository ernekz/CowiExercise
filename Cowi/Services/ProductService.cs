using Cowi.Models;
using Cowi.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace Cowi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Product> CreateProductAsync(Product product)
        {
           return _productRepository.CreateProductAsync(product);
        }

        public Task<bool> DeleteProductAsync(int productId)
        {
            return _productRepository.DeleteProductAsync(productId);
        }

        public Task<Product> GetProductByIdAsync(int productId)
        {
            return _productRepository.GetProductByIdAsync(productId);
        }

        public async Task<Product> UpdateProductAsync(int productId, Product product)
        {
            return await _productRepository.UpdateProductAsync(productId, product);   
        }
    }
}
