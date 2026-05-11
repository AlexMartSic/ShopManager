using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Customer;
using System.Net;

namespace ShopManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService ??
                throw new ArgumentNullException(nameof(customerService));
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerRequestDto requestDto)
        {
            var result = await _customerService.CreateCustomerAsync(requestDto);

            return result.Match<IActionResult>(
                customer => Created($"api/Customer/GetCustomerByCode/{result.Value.Code}", result.Value),
                errors => HandleErrors(errors)
            );
        }

        [HttpDelete("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomerAsync([FromQuery] string code)
        {
            var result = await _customerService.DeleteCustomerAsync(code);

            return result.Match<IActionResult>(
                _ => NoContent(),
                errors => HandleErrors(errors)
            );
        }

        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var result = await _customerService.GetAllCustomersAsync();

            return result.Match<IActionResult>(
                customers => Ok(customers),
                errors => HandleErrors(errors)
            );
        }

        [HttpGet("GetCustomerByCode")]
        public async Task<IActionResult> GetCustomerByCodeAsync([FromQuery] string code)
        {
            var result = await _customerService.GetCustomerByCodeAsync(code);

            return result.Match<IActionResult>(
                customer => Ok(customer),
                errors => HandleErrors(errors)
            );
        }

        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomerAsync([FromBody] CustomerRequestUpdateDto requestDto, string code)
        {
            var result = await _customerService.UpdateCustomerAsync(requestDto, code);

            return result.Match<IActionResult>(
                customer => Ok(customer),
                errors => HandleErrors(errors)
            );
        }
    }
}
