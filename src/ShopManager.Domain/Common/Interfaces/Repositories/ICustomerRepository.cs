using ShopManager.Domain.Entities;

namespace ShopManager.Domain.Common.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        public Task<Customer> CreateCustomerAsync(Customer customer);
        public Task DeleteCustomerAsync(string code);
        public Task<List<Customer>> GetAllCustomersAsync();
        public Task<Customer?> GetCustomerByCodeAsync(string code);
        public Task<Customer> UpdateCustomerAsync(Customer customer);
    }
}
