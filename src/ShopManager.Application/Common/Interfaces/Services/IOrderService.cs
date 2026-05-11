using ErrorOr;
using ShopManager.Application.DTOs.Order;

namespace ShopManager.Application.Common.Interfaces.Services
{
    public interface IOrderService
    {
        public Task<ErrorOr<OrderResponseDetailsDto>> CreateOrderAsync(OrderLineRequestCreateDto requestDto, string customerCode);
        public Task<ErrorOr<Deleted>> DeleteOrderAsync(string orderCode, int lineNumber = 0);
        public Task<List<OrderResponseDto>> GetAllOrdersAsync();
        public Task<ErrorOr<OrderResponseDetailsDto>> GetOrderByCode(string code);
        public Task<ErrorOr<OrderResponseDetailsDto>> UpdateOrderLineQuantityAsync(string orderCode, int lineNumber, int quantity);
    }
}
