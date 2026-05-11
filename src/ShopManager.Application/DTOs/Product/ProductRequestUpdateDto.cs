namespace ShopManager.Application.DTOs.Product
{
    public class ProductRequestUpdateDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
