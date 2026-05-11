using System.ComponentModel.DataAnnotations;

namespace ShopManager.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public required string Code { get; set; }

        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
