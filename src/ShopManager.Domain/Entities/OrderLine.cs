using System.ComponentModel.DataAnnotations;

namespace ShopManager.Domain.Entities
{
    public class OrderLine
    {
        public int Id { get; set; }
        public required int LineNumber { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
