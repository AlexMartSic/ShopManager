using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Product;

namespace ShopManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService ??
                throw new ArgumentNullException(nameof(productService));
        }

        [HttpPut("AddStock")]
        public async Task<IActionResult> AddStockAsync([FromQuery] string productCode, int quantity)
        {
            var result = await _productService.AddStockAsync(productCode, quantity);

            return result.Match<IActionResult>(
                result => Ok(result),
                errors => HandleErrors(errors)
            );
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductRequestCreateDto productDto)
        {
            var result = await _productService.CreateProductAsync(productDto);

            return result.Match(
                product => Created($"api/product/GetProductByCode/{result.Value.Code}", result.Value),
                errors => HandleErrors(errors)
            );

            //return Ok("Produt created.");
        }

        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProductAsync([FromQuery] string code)
        {
            var result = await _productService.DeleteProductAsync(code);

            return result.Match<IActionResult>(
                _ => NoContent(),
                errors => HandleErrors(errors)
            );
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        [HttpGet("GetProductByCode")]
        public async Task<IActionResult> GetProductByCodeAsync([FromQuery] string code)
        {
            var result = await _productService.GetProductByCodeAsync(code);
            return result.Match(
                product => Ok(product),
                errors => HandleErrors(errors)
            );
        }

        [HttpPut("RemoveStock")]
        public async Task<IActionResult> RemoveStockAsync([FromQuery] string productCode, int quantity)
        {
            var result = await _productService.RemoveStockAsync(productCode, quantity);

            return result.Match<IActionResult>(
                result => Ok(result),
                errors => HandleErrors(errors)
            );
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProductAsync([FromBody] ProductRequestUpdateDto productDto, string code)
        {
            var result = await _productService.UpdateProductAsync(productDto, code);

            return result.Match<IActionResult>(
                product => Ok(product),
                errors => HandleErrors(errors)
            );
        }
    }
}
