using ErrorOr;
using FluentAssertions;
using Moq;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Customer;
using ShopManager.Application.Services;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;

namespace ShopManager.UnitTests
{
    public class CustomerServiceTest
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomerService _customerService;

        public CustomerServiceTest()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_customerRepositoryMock.Object);
        }

        #region Get all customers
        [Fact]
        public async Task GetAllCustomers_WhenDoesntExistAnyCustomer_ShouldReturnEmptyList()
        {
            _customerRepositoryMock.Setup(r => r.GetAllCustomersAsync())
                                  .ReturnsAsync(new List<Customer>());

            var result = await _customerService.GetAllCustomersAsync();

            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAllCustomers_WhenCustomersExitstsInDB_ShouldReturnListOfCustomers()
        {
            var customers = new List<Customer>
                                  {
                                      new Customer { Code = "Cust01", Name = "Customer01", Phone = "123456"},
                                      new Customer { Code = "Cust02", Name = "Customer02", Phone = "654321"}
                                  };

            _customerRepositoryMock.Setup(r => r.GetAllCustomersAsync())
                                  .ReturnsAsync(customers);

            var result = await _customerService.GetAllCustomersAsync();

            result.Count.Should().Be(2);
        }
        #endregion

        #region Update customer
        [Fact]
        public async Task UpdateCustomer_WhenAllIsCorrect_ShouldReturnCustomer()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("Cust1"))
                                   .ReturnsAsync(new Customer { Code = "Cust1", Name = "Customer1", Phone = "123456" });

            _customerRepositoryMock.Setup(r => r.UpdateCustomerAsync(It.IsAny<Customer>()))
                                   .ReturnsAsync(new Customer { Code = "Cust1", Name = "Customer2", Phone = "1234567" });

            var result = await _customerService.UpdateCustomerAsync(new CustomerRequestUpdateDto { Name = "Customer2", Phone = "1234567" }, "Cust1");

            result.IsError.Should().BeFalse();
            result.Value.Code.Should().Be("Cust1");
            result.Value.Name.Should().Be("Customer2");
            result.Value.Phone.Should().Be("1234567");
        }

        [Fact]
        public async Task UpdateCustomer_WhenCustomerDoesntExist_ShouldReturnNotFound()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("DOESNTEXIST"))
                                   .ReturnsAsync((Customer?)null);

            var result = await _customerService.UpdateCustomerAsync(new CustomerRequestUpdateDto { Name = "Customer01", Phone = "1234567" }, "DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }
        #endregion

        #region Delete customer
        [Fact]
        public async Task DeleteCustomer_WhenCustomerExists_ShouldReturnDeleted()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("Cust1"))
                                   .ReturnsAsync(new Customer { Code = "Cust1", Name = "Customer1", Phone = "123456" });

            _customerRepositoryMock.Setup(r => r.DeleteCustomerAsync(It.IsAny<Customer>()))
                                   .Returns(Task.CompletedTask);

            var result = await _customerService.DeleteCustomerAsync("Cust1");

            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Deleted);
        }

        [Fact]
        public async Task DeleteCustomer_WhenCustomerDoesntExist_ShouldReturnNotFound()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("DOESNTEXIST"))
                                   .ReturnsAsync((Customer?)null);

            var result = await _customerService.DeleteCustomerAsync("DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }
        #endregion

        #region Get customer by code
        [Fact]
        public async Task GetCustomerByCode_WhenCustomerExists_ShouldReturnCustomer()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("Cust1"))
                                   .ReturnsAsync(new Customer { Code = "Cust1", Name = "Customer1", Phone = "123456" });

            var result = await _customerService.GetCustomerByCodeAsync("Cust1");

            result.IsError.Should().BeFalse();
            result.Value.Code.Should().Be("Cust1");
            result.Value.Name.Should().Be("Customer1");
            result.Value.Phone.Should().Be("123456");
        }

        [Fact]
        public async Task GetCustomerByCode_WhenCustomerDoesntExist_ShouldReturnNotFound()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("DOESNTEXIST"))
                                   .ReturnsAsync((Customer?)null);

            var result = await _customerService.GetCustomerByCodeAsync("DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }
        #endregion

        #region Create customer
        [Fact]
        public async Task CreateCustomer_WhenAllIsCorrect_ShouldReturnCustomer()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("Cust1"))
                                   .ReturnsAsync((Customer?)null);

            _customerRepositoryMock.Setup(r => r.CreateCustomerAsync(It.IsAny<Customer>()))
                                   .ReturnsAsync(new Customer { Code = "Cust1", Name = "Customer1", Phone = "123456" });

            var result = await _customerService.CreateCustomerAsync(new CustomerRequestDto { Code = "Cust1", Name = "Customer1", Phone = "123456" });

            result.IsError.Should().BeFalse();
            result.Value.Code.Should().Be("Cust1");
            result.Value.Name.Should().Be("Customer1");
            result.Value.Phone.Should().Be("123456");
        }

        [Fact]
        public async Task CreateCustomer_WhenCustomerExists_ShouldReturnConflictError()
        {
            _customerRepositoryMock.Setup(r => r.GetCustomerByCodeAsync("Cust1"))
                                   .ReturnsAsync(new Customer { Code = "Cust1", Name = "Customer1", Phone = "123456" });

            var result = await _customerService.CreateCustomerAsync(new CustomerRequestDto { Code = "Cust1", Name = "Customer1", Phone = "123456" });

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Conflict);
        }

        [Theory]
        [InlineData("", "Customer1", "123456")]
        [InlineData("Cust1", "", "123456")]
        public async Task CreateCustomer_WhenFieldsAreEmpty_ShouldReturnValidationError(string dataCode, string dataName, string dataPhone)
        {
            var result = await _customerService.CreateCustomerAsync(new CustomerRequestDto { Code = dataCode, Name = dataName, Phone = dataPhone });

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
        }
        #endregion

    }
}
