using ShopManager.Domain.Entities;
using ShopManager.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ShopManager.Application.DTOs.Order
{
    public class OrderResponseDetailsDto
    {
        public required string Code { get; set; }
        public decimal TotalPrice { get; set; }

        [Required]
        public required string CustomerCode { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderLineResponseDto>? Lines { get; set; }
    }
}
