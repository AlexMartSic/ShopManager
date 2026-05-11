using System.ComponentModel.DataAnnotations;

namespace ShopManager.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Code { get; set; }

        [Required]
        public required string Name { get; set; }
        public string? Phone { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
