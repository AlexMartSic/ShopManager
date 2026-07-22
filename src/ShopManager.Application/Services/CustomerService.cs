using ErrorOr;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Customer;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;

namespace ShopManager.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ??
                throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<ErrorOr<CustomerResponseDto>> CreateCustomerAsync(CustomerRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Code))
            {
                return Error.Validation("Customer.Code", "The customer code cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(requestDto.Name))
            {
                return Error.Validation("Customer.Name", "The customer name cannot be empty.");
            }

            Customer? customer = await _customerRepository.GetCustomerByCodeAsync(requestDto.Code);
            if (customer != null)
            {
                return Error.Conflict("Customer.Code", "A customer with this code already exists.");
            }

            customer = new Customer()
            {
                Code = requestDto.Code,
                Name = requestDto.Name,
                Phone = !string.IsNullOrWhiteSpace(requestDto.Phone) ? requestDto.Phone : string.Empty
            };

            var result = await _customerRepository.CreateCustomerAsync(customer);

            return new CustomerResponseDto
            {
                Code = result.Code,
                Name = result.Name,
                Phone = result.Phone
            };
        }

        public async Task<ErrorOr<Deleted>> DeleteCustomerAsync(string code)
        {
            Customer? customer = await _customerRepository.GetCustomerByCodeAsync(code);
            if (customer == null)
            {
                return Error.NotFound("Customer.Code", "A customer with this code doesn't exist.");
            }

            await _customerRepository.DeleteCustomerAsync(customer);

            return Result.Deleted;
        }

        public async Task<List<CustomerResponseDto>> GetAllCustomersAsync()
        {
            List<Customer> lstCustomers = await _customerRepository.GetAllCustomersAsync();

            if (lstCustomers == null || !lstCustomers.Any())
            {
                return new List<CustomerResponseDto>();
            }

            List<CustomerResponseDto> lstResponse = lstCustomers
                                                    .Select(c => new CustomerResponseDto
                                                    {
                                                        Code = c.Code,
                                                        Name = c.Name,
                                                        Phone = c.Phone
                                                    })
                                                    .ToList();

            return lstResponse;
        }

        public async Task<ErrorOr<CustomerResponseDto>> GetCustomerByCodeAsync(string code)
        {
            var customer = await _customerRepository.GetCustomerByCodeAsync(code);
            if (customer == null)
            {
                return Error.NotFound("Customer.Code", "A customer with this code doesn't exist.");
            }

            return new CustomerResponseDto
            {
                Code = customer.Code,
                Name = customer.Name,
                Phone = customer.Phone
            };
        }

        public async Task<ErrorOr<Customer>> GetCustomerEntityByCodeAsync(string code)
        {
            var customer = await _customerRepository.GetCustomerByCodeAsync(code);
            if (customer == null)
            {
                return Error.NotFound("Customer.Code", "A customer with this code doesn't exist.");
            }

            return customer;
        }

        public async Task<ErrorOr<CustomerResponseDto>> UpdateCustomerAsync(CustomerRequestUpdateDto requestDto, string code)
        {
            Customer? customer = null;

            customer = await _customerRepository.GetCustomerByCodeAsync(code);
            if (customer == null)
            {
                return Error.NotFound("Customer.Code", "A customer with this code doesn't exist.");
            }

            customer.Name = !string.IsNullOrWhiteSpace(requestDto.Name) ? requestDto.Name : customer.Name;
            customer.Phone = !string.IsNullOrWhiteSpace(requestDto.Phone) ? requestDto.Phone : customer.Phone;

            var result = await _customerRepository.UpdateCustomerAsync(customer);

            return new CustomerResponseDto
            {
                Code = result.Code,
                Name = result.Name,
                Phone = result.Phone
            };
        }

    }
}
