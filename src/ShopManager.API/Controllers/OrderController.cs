using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Order;

namespace ShopManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService ??
                throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderLineRequestCreateDto requestDto, string customerCode)
        {
            var result = await _orderService.CreateOrderAsync(requestDto, customerCode);

            return result.Match(
                order => Created($"api/Customer/GetCustomerByCode/{result.Value.Code}", result.Value),
                errors => HandleErrors(errors)
            );
        }

        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> DeleteOrderAsync([FromQuery] string orderCode, int lineNumber)
        {
            var result = await _orderService.DeleteOrderAsync(orderCode, lineNumber);

            return result.Match<IActionResult>(
                _ => NoContent(),
                errors => HandleErrors(errors)
            );
        }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var result = await _orderService.GetAllOrdersAsync();

            return Ok(result);
        }

        [HttpGet("GetOrderByCode")]
        public async Task<IActionResult> GetOrderByCode([FromQuery] string code)
        {
            var result = await _orderService.GetOrderByCode(code);

            return result.Match(
                order => Ok(order),
                errors => HandleErrors(errors)
            );
        }

        [HttpPut("UpdateOrderLineQuantity")]
        public async Task<IActionResult> UpdateOrderLineQuantityAsync([FromQuery] string orderCode,
                                                                                  int lineNumber,
                                                                                  int quantity)
        {
            var result = await _orderService.UpdateOrderLineQuantityAsync(orderCode, lineNumber, quantity);

            return result.Match(
                order => Ok(order),
                errors => HandleErrors(errors)
            );
        }
    }
}
