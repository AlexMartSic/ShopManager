using ErrorOr;
using FluentAssertions;
using Moq;
using ShopManager.Application.DTOs.Product;
using ShopManager.Application.Services;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;

namespace OrderManagementApi.UnitTests
{
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }

        #region Stock
        [Fact]
        public async Task AddStock_WhenAllDataIsCorrect_ShoulReturnString()
        {

        }
        #endregion

        #region Update product
        [Fact]
        public async Task UpdateProduct_WhenAllDataIsCorrect_ShouldReturnProductDto()
        {
            _productRepositoryMock.Setup(r => r.GetProductByCodeAsync("P001"))
                                  .ReturnsAsync(new Product { Code = "P001", Name = "Product01", Price = 5, Stock = 10 });

            _productRepositoryMock.Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                                  .ReturnsAsync(new Product { Code = "P001", Name = "Product01", Price = 5, Stock = 10 });

            var result = await _productService.UpdateProductAsync(new ProductRequestUpdateDto { Name = "Product01", Price = 5 }, "P001");

            result.IsError.Should().BeFalse();
            result.Value.Code.Should().Be("P001");
            result.Value.Name.Should().Be("Product01");
            result.Value.Price.Should().Be(5);
            result.Value.Stock.Should().Be(10);
        }

        [Fact]
        public async Task UpdateProduct_WhenCodeIsEmpty_ShouldReturnValidationError()
        {

            var result = await _productService.UpdateProductAsync(new ProductRequestUpdateDto { Name = "Product01", Price = 5 }, string.Empty);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task UpdateProduct_WhenProductDoesntExist_ShouldReturnNotFound()
        {
            _productRepositoryMock.Setup(r => r.GetProductByCodeAsync("DOESNTEXIST"))
                                  .ReturnsAsync((Product?)null);

            var result = await _productService.UpdateProductAsync(It.IsAny<ProductRequestUpdateDto>(), "DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }
        #endregion

        #region Create product
        [Fact]
        public async Task CreateProduct_WhenAllFieldsAreCorrect_ShouldReturnProduct()
        {
            _productRepositoryMock.Setup(r => r.GetProductByCodeAsync("P001"))
                                  .ReturnsAsync((Product?)null);

            _productRepositoryMock.Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
                                  .ReturnsAsync(new Product { Code = "P001", Name = "Product01", Price = 5, Stock = 10 });

            var result = await _productService.CreateProductAsync(new ProductRequestCreateDto { Code = "P001", Name = "Product01", Price = 5, Stock = 10 });

            result.IsError.Should().BeFalse();
            result.Value.Code.Should().Be("P001");
            result.Value.Name.Should().Be("Product01");
            result.Value.Price.Should().Be(5);
            result.Value.Stock.Should().Be(10);
        }

        [Theory]
        [InlineData("", "Product01", 5, 10)]
        [InlineData("P001", "", 5, 10)]
        [InlineData("P001", "Product01", -1, 10)]
        [InlineData("P001", "Product01", 5, -1)]
        public async Task CreateProduct_WhenSomeFieldsAreEmpty_ShouldReturnValidationError(string codeReq, string nameReq, decimal priceReq, int stockReq)
        {
            var request = new ProductRequestCreateDto { Code = codeReq, Name = nameReq, Price = priceReq, Stock = stockReq };
            var result = await _productService.CreateProductAsync(request);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
        }
        #endregion

        #region Delete product
        [Fact]
        public async Task DeleteProduct_WhenProductExists_ShouldReturnResultDeleted()
        {
            _productRepositoryMock.Setup(r => r.GetProductByCodeAsync("P001"))
                                  .ReturnsAsync(new Product { Code = "P001", Name = "Product01", Price = 5, Stock = 10 });

            _productRepositoryMock.Setup(r => r.DeleteProductAsync(It.IsAny<Product>()))
                                  .Returns(Task.CompletedTask);



            var result = await _productService.DeleteProductAsync("P001");

            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Deleted);
        }

        [Fact]
        public async Task DeleteProduct_WhenProductDoesntExist_ShouldReturnNotFound()
        {
            _productRepositoryMock.Setup(r => r.GetProductByCodeAsync("DOESNTEXIST"))
                                  .ReturnsAsync((Product?)null);


            var result = await _productService.DeleteProductAsync("DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }
        #endregion

        #region Get all products
        [Fact]
        public async Task GetAllProducts_WhenDoesntExistAnyProduct_ShouldReturnEmptyList()
        {
            _productRepositoryMock.Setup(r => r.GetAllProductsAsync())
                                  .ReturnsAsync(new List<Product>());

            var result = await _productService.GetAllProductsAsync();

            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAllProducts_WhenProductsExitstsInDB_ShouldReturnListOfProducts()
        {
            var products = new List<Product>
                                  {
                                      new Product { Code = "P001", Name = "Product01", Price = 10, Stock = 20 },
                                      new Product { Code = "P002", Name = "Product02", Price = 15, Stock = 23 }
                                  };

            _productRepositoryMock.Setup(r => r.GetAllProductsAsync())
                                  .ReturnsAsync(products);

            var result = await _productService.GetAllProductsAsync();

            result.Count.Should().Be(2);
        }
        #endregion

        #region Get product by code
        [Fact]
        public async Task GetProductByCode_WhenProductDoesntExist_ShouldReturnNotFound()
        {
            _productRepositoryMock.Setup(r => r.GetProductByCodeAsync("DOESNTEXIST"))
                                  .ReturnsAsync((Product?)null);

            var result = await _productService.GetProductByCodeAsync("DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task GetProductByCode_WhenProductExists_ShouldReturnProduct()
        {
            _productRepositoryMock.Setup(r => r.GetProductByCodeAsync("P001"))
                                  .ReturnsAsync(new Product { Code = "P001", Name = "Product01", Price = 10, Stock = 20 });

            var result = await _productService.GetProductByCodeAsync("P001");

            result.IsError.Should().BeFalse();
            result.Value.Code.Should().Be("P001");
            result.Value.Name.Should().Be("Product01");
            result.Value.Price.Should().Be(10);
            result.Value.Stock.Should().Be(20);
        }
        #endregion
    }
}