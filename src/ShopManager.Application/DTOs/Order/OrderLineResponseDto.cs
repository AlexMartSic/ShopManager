using System.ComponentModel.DataAnnotations;

namespace ShopManager.Application.DTOs.Order
{
    public class OrderLineResponseDto
    {
        public required int LineNumber { get; set; }

        [Required]
        public required string ProductCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
