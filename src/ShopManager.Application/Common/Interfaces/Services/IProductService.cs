using ErrorOr;
using ShopManager.Application.DTOs.Product;

namespace ShopManager.Application.Common.Interfaces.Services
{
    public interface IProductService
    {
        Task<ErrorOr<string>> AddStockAsync(string productCode, int quantity);
        Task<ErrorOr<ProductResponseDto>> CreateProductAsync(ProductRequestCreateDto requestDto);
        Task<ErrorOr<Deleted>> DeleteProductAsync(string code);
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ErrorOr<ProductResponseDto>> GetProductByCodeAsync(string code);
        Task<ErrorOr<string>> RemoveStockAsync(string productCode, int quantity);
        Task<ErrorOr<ProductResponseDto>> UpdateProductAsync(ProductRequestUpdateDto requestDto, string code);
    }
}
