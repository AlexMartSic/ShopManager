using Microsoft.EntityFrameworkCore;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;
using ShopManager.Infrastructure.Data;

namespace ShopManager.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ??
                throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Product> CreateProductAsync(Product product)
        {


            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task DeleteProductAsync(Product product)
        {
            _dbContext.Remove(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByCodeAsync(string code)
        {
            string errorMsg = string.Empty;

            Product? product = await _dbContext.Products.Where(p => p.Code.ToUpper().Trim() == code.ToUpper().Trim())
                                               .FirstOrDefaultAsync();
            if (product == null)
            {
                return null;
            }

            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();

            return product;
        }
    }
}
