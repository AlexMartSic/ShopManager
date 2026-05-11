using ErrorOr;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Order;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;
using ShopManager.Domain.Enumerations;

namespace ShopManager.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;

        public OrderService(IOrderRepository orderRepository,
                            ICustomerRepository customerRepository,
                            IProductRepository productRepository,
                            IProductService productService)
        {
            _orderRepository = orderRepository ??
                throw new ArgumentNullException(nameof(orderRepository));

            _customerRepository = customerRepository ??
                throw new ArgumentNullException(nameof(customerRepository));

            _productRepository = productRepository ??
                throw new ArgumentNullException(nameof(productRepository));

            _productService = productService ??
                throw new ArgumentNullException(nameof(productService));
        }

        public async Task<ErrorOr<OrderResponseDetailsDto>> CreateOrderAsync(OrderLineRequestCreateDto requestDto, string customerCode)
        {
            var customer = await _customerRepository.GetCustomerByCodeAsync(customerCode);
            if (customer == null)
            {
                return Error.NotFound("Customer.Code", "A customer with this code doesn't exist.");
            }

            var product = await _productRepository.GetProductByCodeAsync(requestDto.ProductCode);
            if (product == null)
            {
                return Error.NotFound("Product.Code", "A product with this code doesn't exist.");
            }

            var order = await _orderRepository.GetOrderByCodeAsync(requestDto.OrderCode);
            if (order != null && !string.Equals(order?.Customer?.Code, customerCode, StringComparison.CurrentCultureIgnoreCase))
            {
                return Error.Validation("Order.Customer.Code", "The order's customer code is different to the requested customer code");
            }

            if (product.Stock < requestDto.Quantity)
            {
                return Error.Validation("Product.Stock", "Insufficient stock of this product.");
            }

            if (order == null)
            {
                order = new Order()
                {
                    Code = requestDto.OrderCode,
                    CustomerId = customer.Id,
                    TotalPrice = 0,
                    Status = OrderStatus.InProgress
                };
            }

            int lineNumber = 1;
            if (order.Lines.Any())
            {
                lineNumber = order.Lines.Max(m => m.LineNumber) + 1;
            }

            OrderLine line = new OrderLine()
            {
                LineNumber = lineNumber,
                ProductId = product.Id,
                Quantity = requestDto.Quantity,
                Price = product.Price * requestDto.Quantity
            };

            order.Lines.Add(line);

            //order.TotalPrice = order.Lines.Sum(s => s.Price);
            order.TotalPrice += product.Price * line.Quantity;

            var result = await _orderRepository.CreateOrderAsync(order);
            var newStock = await _productService.RemoveStockAsync(requestDto.ProductCode, line.Quantity);

            OrderResponseDetailsDto response = new OrderResponseDetailsDto()
            {
                Code = result.Code,
                CustomerCode = result.Customer!.Code,
                TotalPrice = result.TotalPrice,
                Status = result.Status,
                Lines = result.Lines.Select(s => new OrderLineResponseDto
                {
                    LineNumber = s.LineNumber,
                    Price = s.Price,
                    ProductCode = s.Product!.Code,
                    Quantity = s.Quantity
                }).ToList()
            };

            return response;
        }

        public async Task<ErrorOr<Deleted>> DeleteOrderAsync(string orderCode, int lineNumber = 0)
        {
            OrderLine? orderLine = null;
            var order = await _orderRepository.GetOrderByCodeAsync(orderCode);
            if (order == null)
            {
                return Error.NotFound("Order.Code", "A order with this code doesn't exist.");
            }

            if (lineNumber != 0)
            {
                orderLine = order.Lines.Where(l => l.LineNumber == lineNumber).FirstOrDefault();
                if (orderLine == null)
                {
                    return Error.NotFound("OrderLine.LineNumber", "This order doesn't have a line with this number.");
                }
            }
            else
            {
                if (order.Lines.Any())
                {
                    return Error.Validation("Order.Lines", "Cannot delete an order with lines. Delete the lines before.");
                }
            }

            if (orderLine != null)
            {
                order.TotalPrice -= orderLine.Price;
                var result = await _orderRepository.UpdateOrderAsync(order);
            }

            await _orderRepository.DeleteOrderAsync(order, orderLine);

            return Result.Deleted;
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            List<Order> result = await _orderRepository.GetAllOrdersAsync();
            if (result == null || !result.Any())
            {
                return new List<OrderResponseDto>();
            }

            var orders = result.Select(o => new OrderResponseDto
            {
                Code = o.Code,
                CustomerCode = o.Customer!.Code,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
            }).ToList();

            return orders;
        }

        public async Task<ErrorOr<OrderResponseDetailsDto>> GetOrderByCode(string code)
        {
            var order = await _orderRepository.GetOrderByCodeAsync(code);

            if (order == null)
            {
                return Error.NotFound("Product.Code", "A product with this code doesn't exist.");
            }

            OrderResponseDetailsDto res = new OrderResponseDetailsDto()
            {
                Code = order.Code,
                TotalPrice = order.TotalPrice,
                CustomerCode = order.Customer!.Code,
                Status = order.Status,
                Lines = order.Lines.Select(s => new OrderLineResponseDto()
                {
                    LineNumber = s.LineNumber,
                    Price = s.Price,
                    ProductCode = s.Product!.Code,
                    Quantity = s.Quantity
                }).ToList()
            };
            return res;
        }

        public async Task<ErrorOr<OrderResponseDetailsDto>> UpdateOrderLineQuantityAsync(string orderCode,
                                                                                    int lineNumber,
                                                                                    int quantity)
        {
            var order = await _orderRepository.GetOrderByCodeAsync(orderCode);
            if (order == null)
            {
                return Error.NotFound("Order.Code", "An order with this code doesn't exist.");
            }

            var orderLine = order.Lines.Where(l => l.LineNumber == lineNumber).FirstOrDefault();
            if (orderLine == null)
            {
                return Error.NotFound("OrderLine.LineNumber", "This order doesn't have a line with this number.");
            }

            var product = orderLine.Product;
            if (product == null)
            {
                return Error.NotFound("Product", "This line doesn't have a product.");
            }
            if (product.Stock < quantity)
            {
                return Error.Validation("Product.Stock", "Insufficient stock of this product.");
            }

            Order result;

            if (orderLine.Quantity < quantity)
            {
                order.TotalPrice -= (quantity - orderLine.Quantity) * product.Price;
                orderLine.Quantity = quantity;
                result = await _orderRepository.UpdateOrderAsync(order, orderLine);
            }
            else
            {
                order.TotalPrice += (orderLine.Quantity - quantity) * product.Price;
                orderLine.Quantity = quantity;
                result = await _orderRepository.UpdateOrderAsync(order, orderLine);
            }

            OrderResponseDetailsDto response = new OrderResponseDetailsDto()
            {
                Code = result.Code,
                CustomerCode = result.Customer!.Code,
                TotalPrice = result.TotalPrice,
                Status = result.Status,
                Lines = result.Lines.Select(l => new OrderLineResponseDto
                {
                    LineNumber = l.LineNumber,
                    Price = l.Price,
                    ProductCode = l.Product!.Code,
                    Quantity = l.Quantity,
                }).ToList()
            };

            return response;
        }
    }
}
