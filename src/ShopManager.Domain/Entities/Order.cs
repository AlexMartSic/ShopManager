using ShopManager.Domain.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ShopManager.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
    }
}
