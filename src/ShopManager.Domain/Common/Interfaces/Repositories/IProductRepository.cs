using ShopManager.Domain.Entities;

namespace ShopManager.Domain.Common.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByCodeAsync(string code);
        Task<Product> UpdateProductAsync(Product productDto);
    }
}
