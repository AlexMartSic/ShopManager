using ErrorOr;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Product;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;

namespace ShopManager.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository ??
                throw new ArgumentNullException(nameof(repository));
        }

        public async Task<ErrorOr<Updated>> AddStockAsync(string productCode, int quantity)
        {
            if (quantity < 0)
            {
                return Error.Validation("Quantity", "The quantity cannot be less than 0.");
            }

            var product = await _repository.GetProductByCodeAsync(productCode);
            if (product == null)
            {
                return Error.NotFound("Product.Code", "A product with this code doesn't exist.");
            }

            product.Stock += quantity;
            var result = await _repository.UpdateProductAsync(product);

            return Result.Updated;
        }

        public async Task<ErrorOr<ProductResponseDto>> CreateProductAsync(ProductRequestCreateDto requestDto)
        {
            string errorMsg = string.Empty;

            //Basic checks
            if (string.IsNullOrWhiteSpace(requestDto.Code))
            {
                return Error.Validation("Product.Code", "The product code cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(requestDto.Name))
            {
                return Error.Validation("Product.Name", "Product.NamThe product name cannot be empty.");
            }
            if (requestDto.Price < 0)
            {
                return Error.Validation("Product.Price", "The price cannot be less than 0.");
            }
            if (requestDto.Stock < 0)
            {
                return Error.Validation("Product.Stock", "The stock cannot be less than 0.");
            }

            //Check that the code of the product doesn't exist
            var productResponse = await _repository.GetProductByCodeAsync(requestDto.Code);
            if (productResponse != null)
            {
                return Error.Conflict("Product.Code", "A product with this code already exists.");
            }

            Product product = new Product
            {
                Code = requestDto.Code,
                Name = requestDto.Name,
                Description = requestDto.Description,
                Price = requestDto.Price,
                Stock = requestDto.Stock
            };

            var result = await _repository.CreateProductAsync(product);

            ProductResponseDto response = new ProductResponseDto
            {
                Code = result.Code,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                Stock = result.Stock
            };

            return response;
        }

        public async Task<ErrorOr<Deleted>> DeleteProductAsync(string code)
        {
            var product = await _repository.GetProductByCodeAsync(code);

            if (product == null)
            {
                return Error.NotFound("Product.Code", "A product with this code doesn't exist");
            }

            await _repository.DeleteProductAsync(product);

            return Result.Deleted;
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            List<Product> lstProd = await _repository.GetAllProductsAsync();

            if (!lstProd.Any() || lstProd == null)
            {
                return new List<ProductResponseDto>();
            }

            return lstProd.Select(p => new ProductResponseDto
            {
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock
            })
            .ToList();
        }

        public async Task<ErrorOr<ProductResponseDto>> GetProductByCodeAsync(string code)
        {
            var productResponse = await _repository.GetProductByCodeAsync(code);
            if (productResponse == null)
            {
                return Error.NotFound("Product.Code", "A product with this code doesn't exist");
            }

            return new ProductResponseDto
            {
                Code = productResponse.Code,
                Name = productResponse.Name,
                Description = productResponse.Description,
                Price = productResponse.Price,
                Stock = productResponse.Stock
            };
        }

        public async Task<ErrorOr<Updated>> RemoveStockAsync(string productCode, int quantity)
        {
            if (quantity < 0)
            {
                return Error.Validation("Quantity", "The quantity cannot be less than 0.");
            }

            var product = await _repository.GetProductByCodeAsync(productCode);
            if (product == null)
            {
                return Error.NotFound("Product.Code", "A product with this code doesn't exist.");
            }

            if (product.Stock < quantity)
            {
                return Error.Validation("Product.Stock", "Not enough stock.");
            }

            product.Stock -= quantity;
            var result = await _repository.UpdateProductAsync(product);

            return Result.Updated;
        }

        public async Task<ErrorOr<ProductResponseDto>> UpdateProductAsync(ProductRequestUpdateDto requestDto, string code)
        {
            Product? product = null;

            if (string.IsNullOrWhiteSpace(code))
            {
                return Error.Validation("Product.Code", "The product code cannot be empty.");
            }

            product = await _repository.GetProductByCodeAsync(code);
            if (product == null)
            {
                return Error.NotFound("Product.Code", "A product with this code does not exist.");
            }

            product.Name = requestDto.Name;
            product.Description = requestDto.Description;
            product.Price = requestDto.Price;

            Product result = await _repository.UpdateProductAsync(product);

            ProductResponseDto response = new ProductResponseDto
            {
                Code = result.Code,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                Stock = result.Stock
            };

            return response;
        }
    }
}
