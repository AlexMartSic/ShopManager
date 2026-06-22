using ErrorOr;
using ShopManager.Application.DTOs.Customer;

namespace ShopManager.Application.Common.Interfaces.Services
{
    public interface ICustomerService
    {
        public Task<ErrorOr<CustomerResponseDto>> CreateCustomerAsync(CustomerRequestDto requestDto);
        public Task<ErrorOr<Deleted>> DeleteCustomerAsync(string code);
        public Task<List<CustomerResponseDto>> GetAllCustomersAsync();
        public Task<ErrorOr<CustomerResponseDto>> GetCustomerByCodeAsync(string code);
        public Task<ErrorOr<CustomerResponseDto>> UpdateCustomerAsync(CustomerRequestUpdateDto requestDto, string code);
    }
}
