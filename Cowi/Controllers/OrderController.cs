
using Cowi.Models;
using Cowi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net.Http.Headers;
using System.Net.Mime;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Cowi.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class OrderController : ControllerBase
    { 

        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Adds a product to the order, if the order does not exist it creates one. 
        /// </summary>
        /// <remarks>
        /// Only Authenticated users can create/add products to the order.
        /// It is being Authenticated using JWT Token.
        /// Inside the JWT Token there is UserId(Email) which is used for creating order.
        /// 
        /// 
        /// </remarks>
        /// <param name="Order"> The user's register credentials</param>
        /// <returns>Returns a message upon successful registration</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProductToOrder(int productId, int quantity)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized("JWT token is missing.");
                var order = await _orderService.AddProductToOrderAsync(productId, quantity, jwtToken);
                return CreatedAtAction(nameof(AddProductToOrder), order);
               // return Ok(true);
            }
            catch (UnauthorizedAccessException ex) 
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Removes the product from Order that User holds. 
        /// </summary>
        /// <remarks>
        /// Only Authenticated users can remove products from the order.
        /// It is being Authenticated using JWT Token.
        /// Inside the JWT Token there is UserId(Email) which is used for finding Users Order.
        /// 
        /// 
        /// </remarks>
        /// <param name="Order"> The user's register credentials</param>
        /// <returns>Returns a message upon successful registration</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveProductFromOrder(int productId)
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized("JWT token is missing.");
                var result = await _orderService.RemoveProductFromOrderAsync(productId, jwtToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Retrieves User Order using JWT Token 
        /// </summary>
        /// <remarks>
        /// 
        /// It is being Authenticated using JWT Token.
        /// Inside the JWT Token there is UserId(Email) which is used for finding Users Order.
        /// 
        /// 
        /// </remarks>
        /// <param name="Order"> The user's register credentials</param>
        /// <returns>Returns a message upon successful registration</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CurrentUserOrder()
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized("JWT token is missing.");
                var result = await _orderService.GetCurrentUserOrderAsync(jwtToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Completes the Order. 
        /// </summary>
        /// <remarks>
        /// Only the User can complete the order.
        /// It is being Authenticated using JWT Token.
        /// Inside the JWT Token there is UserId(Email) which is used for finding Users Order.
        /// Order retrieved and Transaction begins.
        /// 
        /// 
        /// </remarks>
        /// <param name="Order"> The user's register credentials</param>
        /// <returns>Returns a message upon successful registration</returns>
        [HttpPut("complete")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteOrder()
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized("JWT token is missing.");
                var result = await _orderService.CompleteOrderAsync(jwtToken);
                return Ok(result);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete the Order. 
        /// </summary>
        /// <remarks>
        /// Only the User can Delete his order.
        /// It is being Authenticated using JWT Token.
        /// Inside the JWT Token there is UserId(Email) which is used for finding Users Order.
        /// If the User has the order it will be Deleted
        /// 
        /// 
        /// </remarks>
        /// <param name="Order"> The user's register credentials</param>
        /// <returns>Returns a message upon successful registration</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteOrder()
        {
            try
            {
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized("JWT token is missing.");
                var result = await _orderService.DeleteOrderAsync(jwtToken);
                return Ok(result);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
