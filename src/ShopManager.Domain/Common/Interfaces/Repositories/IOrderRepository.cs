using ShopManager.Domain.Entities;

namespace ShopManager.Domain.Common.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        public Task<Order> CreateOrderAsync(Order requestDto);
        public Task DeleteOrderAsync(Order order, OrderLine? line = null);
        public Task<List<Order>> GetAllOrdersAsync();
        public Task<Order?> GetOrderByCodeAsync(string code);
        public Task<Order> UpdateOrderAsync(Order order, OrderLine? orderLine = null);
    }
}
