using System.ComponentModel.DataAnnotations;

namespace ShopManager.Application.DTOs.Customer
{
    public class CustomerRequestUpdateDto
    {
        [Required]
        public required string Name { get; set; }
        public string? Phone { get; set; }
    }
}
