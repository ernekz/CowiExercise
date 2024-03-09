
using Cowi.Models;
using Cowi.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net.Mime;

namespace Cowi.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// You can create a Product here.
        /// </summary>
        /// <remarks>
        /// All the parameters in the request body cannot be null
        /// 
        /// Sample request:
        /// 
        ///     POST /Product
        ///     {
        ///         "name": "iPhone 13 Pro",
        ///         "description": "256GB Fully loaded iPhone",
        ///         "price": 999.99,
        ///         "stockQuantity": 50
        ///     }
        /// </remarks>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> CreateProductAsync(Product product)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction("GetProductById", new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return appropriate error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// You can create retrieve the Product by id.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        /// <summary>
        /// You can update a Product here by ID.
        /// </summary>
        /// <remarks>
        /// You can change all information about the Product here.
        /// 
        /// NOTE: Parameters cannot be null.
        /// 
        /// Sample request:
        /// 
        ///     PUT /Product/{id}
        ///     {
        ///         "name": "iPhone 13 Pro",
        ///         "description": "64GB Mini loaded iPhone",
        ///         "price": 700.99,
        ///         "stockQuantity": 12
        ///     }
        /// </remarks>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> UpdateProductAsync(int id, Product product)
        {

            try
            {

                var updatedProduct = await _productService.UpdateProductAsync(id, product);
                if (updatedProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return Ok(updatedProduct);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// You can delete a Product here by ID.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            try
            {
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                var success = await _productService.DeleteProductAsync(id);
                if (!success)
                {
                    return StatusCode(500, $"Failed to delete product with ID {id}");
                }
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
