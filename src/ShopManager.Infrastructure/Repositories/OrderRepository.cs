using Microsoft.EntityFrameworkCore;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;
using ShopManager.Infrastructure.Data;

namespace ShopManager.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ??
                throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (!_dbContext.Orders.Contains(order))
            {
                await _dbContext.Orders.AddAsync(order);
                await _dbContext.SaveChangesAsync();
            }


            foreach (var line in order.Lines.Where(l => l.Order == null))
            {
                await _dbContext.OrderLines.AddAsync(line);
            }
            await _dbContext.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrderAsync(Order order, OrderLine? line = null)
        {
            if (line != null)
            {
                _dbContext.Remove(line);
            }
            else
            {
                _dbContext.Remove(order);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = await _dbContext.Orders.Include(o => o.Customer)
                                                .ToListAsync();

            return orders;
        }

        public async Task<Order?> GetOrderByCodeAsync(string code)
        {
            var order = await _dbContext.Orders.Where(o => o.Code.ToUpper().Trim() == code.ToUpper().Trim())
                                               .Include(i => i.Customer)
                                               .Include(i => i.Lines)
                                                    .ThenInclude(i => i.Product)
                                               .FirstOrDefaultAsync();

            return order;

        }

        public async Task<Order> UpdateOrderAsync(Order order, OrderLine? orderLine = null)
        {
            if (orderLine != null)
            {
                _dbContext.OrderLines.Update(orderLine);
            }

            _dbContext.Orders.Update(order);

            await _dbContext.SaveChangesAsync();

            return order;
        }
    }
}
