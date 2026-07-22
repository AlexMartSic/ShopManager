using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopManager.Domain.Entities
{
    public class OrderLine
    {
        public int Id { get; set; }

        [Required]
        public required int LineNumber { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [DefaultValue(1)]
        public required int Quantity { get; set; }

        [DefaultValue(1)]
        public required decimal Price { get; set; }
    }
}
