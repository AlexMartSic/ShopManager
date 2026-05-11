using System.ComponentModel.DataAnnotations;

namespace ShopManager.Application.DTOs.Order
{
    public class OrderLineRequestCreateDto
    {
        [Required]
        public required string OrderCode { get; set; }

        [Required]
        public required string ProductCode { get; set; }
        public int Quantity { get; set; }
    }
}
