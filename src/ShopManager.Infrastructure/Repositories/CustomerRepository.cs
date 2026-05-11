using Microsoft.EntityFrameworkCore;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;
using ShopManager.Infrastructure.Data;

namespace ShopManager.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _dbContext;

        public CustomerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ??
                throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return customer;
        }

        public async Task DeleteCustomerAsync(string code)
        {
            Customer? customer = await _dbContext.Customers.Where(c => c.Code.ToUpper().Trim() == code.ToUpper().Trim())
                                                           .FirstOrDefaultAsync();

            if (customer != null)
                _dbContext.Customers.Remove(customer);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByCodeAsync(string code)
        {
            Customer? customer = await _dbContext.Customers.Where(c => c.Code.ToUpper().Trim() == code.ToUpper().Trim())
                                                          .FirstOrDefaultAsync();

            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            await _dbContext.SaveChangesAsync();

            return customer;
        }
    }
}
