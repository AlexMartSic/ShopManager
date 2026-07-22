using ErrorOr;
using ShopManager.Application.DTOs.Product;
using ShopManager.Domain.Entities;

namespace ShopManager.Application.Common.Interfaces.Services
{
    public interface IProductService
    {
        Task<ErrorOr<Updated>> AddStockAsync(string productCode, int quantity);
        Task<ErrorOr<ProductResponseDto>> CreateProductAsync(ProductRequestCreateDto requestDto);
        Task<ErrorOr<Deleted>> DeleteProductAsync(string code);
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ErrorOr<ProductResponseDto>> GetProductByCodeAsync(string code);
        Task<ErrorOr<Product>> GetProductEntityByCodeAsync(string code);
        Task<ErrorOr<Updated>> RemoveStockAsync(string productCode, int quantity);
        Task<ErrorOr<ProductResponseDto>> UpdateProductAsync(ProductRequestUpdateDto requestDto, string code);
    }
}
