using System.ComponentModel.DataAnnotations;

namespace ShopManager.Application.DTOs.Customer
{
    public class CustomerRequestDto
    {
        [Required]
        public required string Code { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Phone { get; set; }
    }
}
