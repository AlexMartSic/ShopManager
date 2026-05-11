namespace ShopManager.Application.DTOs.Product
{
    public class ProductRequestCreateDto
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
