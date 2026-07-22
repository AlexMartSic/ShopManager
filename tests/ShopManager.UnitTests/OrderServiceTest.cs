using ErrorOr;
using FluentAssertions;
using Moq;
using ShopManager.Application.Common.Interfaces.Services;
using ShopManager.Application.DTOs.Order;
using ShopManager.Application.Services;
using ShopManager.Domain.Common.Interfaces.Repositories;
using ShopManager.Domain.Entities;
using ShopManager.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopManager.UnitTests
{
    public class OrderServiceTest
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly OrderService _orderService;
        private readonly Mock<IProductService> _productServiceMock;

        public OrderServiceTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _customerServiceMock = new Mock<ICustomerService>();

            //_productService = new ProductService(_productRepositoryMock.Object);
            _productServiceMock = new Mock<IProductService>();

            _orderService = new OrderService(_orderRepositoryMock.Object,
                                             _customerServiceMock.Object,
                                             _productServiceMock.Object);
        }

        #region Static methods
        private static Order CreateOrder(string code, string custCode, string custName, string custPhone,
                                   string prodCode, string prodName, decimal price, int quantity)
        {
            return new Order
            {
                Code = code,
                Customer = new Customer
                {
                    Code = custCode,
                    Name = custName,
                    Phone = custPhone
                },
                Lines = new List<OrderLine>
                {
                    new OrderLine { LineNumber = 1, Quantity = quantity, Price = price * quantity, Product = new Product { Code = prodCode,
                                                                                                 Name = prodName,
                                                                                                 Price = price,
                                                                                                 Stock = 10 } }
                },
                TotalPrice = price * quantity,
                Status = OrderStatus.InProgress
            };
        }
        #endregion

        #region Update order line quantity

        //InlineData for lower (1) and higher (3) values
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task UpdateOrderLineQuantity_WhenAllIsCorrect_ShouldReturnOrder(int quantity)
        {
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                                 .ReturnsAsync(order);

            _orderRepositoryMock.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>(), It.IsAny<OrderLine>()))
                                .ReturnsAsync(order);

            var expectedDto = new OrderResponseDetailsDto()
            {
                Code = "Order01",
                CustomerCode = "Cust01",
                Status = OrderStatus.InProgress,
                Lines = new List<OrderLineResponseDto>
                {
                    new OrderLineResponseDto { LineNumber = 1, ProductCode = "Prod01", Quantity = quantity, Price = 10 * quantity }
                },
                TotalPrice = 10 * quantity
            };

            var result = await _orderService.UpdateOrderLineQuantityAsync("Order01", 1, quantity);

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedDto);

            _orderRepositoryMock.Verify(r => r.UpdateOrderAsync(It.IsAny<Order>(), It.IsAny<OrderLine>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderLineQuantity_WhenOrderDoesntExist_ShouldNotFound()
        {

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("DOESNTEXIST"))
                                 .ReturnsAsync((Order?)null);

            var result = await _orderService.UpdateOrderLineQuantityAsync("DOESNTEXIST", 1, 3);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("Order.Code");
        }

        [Fact]
        public async Task UpdateOrderLineQuantity_WhenLineNumDoesntExist_ShouldReturnNotFound()
        {
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                                 .ReturnsAsync(order);

            var result = await _orderService.UpdateOrderLineQuantityAsync("Order01", 5, 3);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("OrderLine.LineNumber");
        }

        [Fact]
        public async Task UpdateOrderLineQuantity_WhenLineDoesntHaveProduct_ShouldReturnValidationError()
        {
            //var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);
            var order = new Order()
            {
                Code = "Order01",
                Customer = new Customer
                {
                    Code = "Cust01",
                    Name = "Customer01",
                    Phone = "123456"
                },
                Lines = new List<OrderLine>
                {
                    new OrderLine { LineNumber = 1, Quantity = 2, Price = 10, Product = null }
                },
                TotalPrice = 20
            };

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                                 .ReturnsAsync(order);

            var result = await _orderService.UpdateOrderLineQuantityAsync("Order01", 1, 3);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
            result.FirstError.Code.Should().Be("Product");
        }

        [Fact]
        public async Task UpdateOrderLineQuantity_WhenStockIsInsufficient_ShouldReturnValidationError()
        {
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                                 .ReturnsAsync(order);

            var result = await _orderService.UpdateOrderLineQuantityAsync("Order01", 1, 20);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
            result.FirstError.Code.Should().Be("Product.Stock");
        }
        #endregion

        #region Get order by code
        [Fact]
        public async Task GetOrderByCode_WhenOrderExists_ShouldReturnOrder()
        {
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                                 .ReturnsAsync(order);

            var expectedDto = new OrderResponseDetailsDto()
            {
                Code = "Order01",
                CustomerCode = "Cust01",
                Status = OrderStatus.InProgress,
                Lines = new List<OrderLineResponseDto>
                {
                    new OrderLineResponseDto { LineNumber = 1, ProductCode = "Prod01", Quantity = 2, Price = 20 }
                },
                TotalPrice = 20
            };

            var result = await _orderService.GetOrderByCode("Order01");

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedDto);

            _orderRepositoryMock.Verify(r => r.GetOrderByCodeAsync("Order01"), Times.Once);
        }

        [Fact]
        public async Task GetOrderByCode_WhenOrderDoesntExist_ShouldReturnNotFound()
        {
            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("DOESNTEXIST"))
                                 .ReturnsAsync((Order?)null);

            var result = await _orderService.GetOrderByCode("DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);

            _orderRepositoryMock.Verify(r => r.GetOrderByCodeAsync("DOESNTEXIST"), Times.Once);
        }
        #endregion

        #region Delete order
        [Fact]
        public async Task DeleteOrderLine_WhenLineExists_ShouldReturnDeleted()
        {
            //Create an order with one line and add a new extra line
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            var lineToDelete = new OrderLine
            {
                LineNumber = 2,
                Quantity = 1,
                Price = 15,
                Product = new Product { Code = "Prod02", Name = "Product02", Price = 15 }
            };

            order.Lines.Add(lineToDelete);
            order.TotalPrice += lineToDelete.Price;

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                              .ReturnsAsync(order);

            _orderRepositoryMock.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>(), It.IsAny<OrderLine>()))
                 .ReturnsAsync(order);

            _orderRepositoryMock.Setup(r => r.DeleteOrderAsync(It.IsAny<Order>(), It.IsAny<OrderLine>()))
                 .Returns(Task.CompletedTask);

            var result = await _orderService.DeleteOrderAsync("Order01", 2);

            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Deleted);

            order.TotalPrice.Should().Be(20);

            _orderRepositoryMock.Verify(r => r.UpdateOrderAsync(order), Times.Once);
            _orderRepositoryMock.Verify(r => r.DeleteOrderAsync(order, lineToDelete), Times.Once);
        }

        [Fact]
        public async Task DeleteOrder_WhenOrderDoesntHaveLines_ShouldReturnDeleted()
        {
            //Create an order with one line and add a new extra line
            var order = new Order
            {
                Code = "Order01",
                Customer = new Customer
                {
                    Code = "Cust01",
                    Name = "Customer01",
                    Phone = "123456"
                },
                Lines = new List<OrderLine>()
            };

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                              .ReturnsAsync(order);

            _orderRepositoryMock.Setup(r => r.DeleteOrderAsync(It.IsAny<Order>()))
                 .Returns(Task.CompletedTask);

            var result = await _orderService.DeleteOrderAsync("Order01");

            result.IsError.Should().BeFalse();
            result.Value.Should().Be(Result.Deleted);

            _orderRepositoryMock.Verify(r => r.DeleteOrderAsync(order), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderLine_WhenOrderDoesntExist_ShouldReturnNotFound()
        {
            //Create an order with one line and add a new extra line
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            var lineToDelete = new OrderLine
            {
                LineNumber = 2,
                Quantity = 1,
                Price = 15,
                Product = new Product { Code = "Prod02", Name = "Product02", Price = 15 }
            };

            order.Lines.Add(lineToDelete);
            order.TotalPrice += lineToDelete.Price;

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("DOESNTEXIST"))
                              .ReturnsAsync((Order?)null);

            var result = await _orderService.DeleteOrderAsync("DOESNTEXIST", 2);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task DeleteOrderLine_WhenLineDoesntExists_ShouldReturnNotFound()
        {
            //Create an order with one line and add a new extra line
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            var lineToDelete = new OrderLine
            {
                LineNumber = 2,
                Quantity = 1,
                Price = 15,
                Product = new Product { Code = "Prod02", Name = "Product02", Price = 15 }
            };

            order.Lines.Add(lineToDelete);
            order.TotalPrice += lineToDelete.Price;

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                              .ReturnsAsync(order);

            var result = await _orderService.DeleteOrderAsync("Order01", 3);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task DeleteOrder_WhenOrderHaveLines_ShouldReturnValidationError()
        {
            //Create an order with one line and add a new extra line
            var order = CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2);

            var lineToDelete = new OrderLine
            {
                LineNumber = 2,
                Quantity = 1,
                Price = 15,
                Product = new Product { Code = "Prod02", Name = "Product02", Price = 15 }
            };

            order.Lines.Add(lineToDelete);
            order.TotalPrice += lineToDelete.Price;

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                              .ReturnsAsync(order);

            var result = await _orderService.DeleteOrderAsync("Order01");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
        }
        #endregion

        #region Create order
        [Fact]
        public async Task CreateOrder_WhenAllIsCorrect_ShouldReturnOrder()
        {
            _customerServiceMock.Setup(s => s.GetCustomerEntityByCodeAsync("Cust01"))
                                   .ReturnsAsync(new Customer { Code = "Cust01", Name = "Customer01", Phone = "123456" });

            _productServiceMock.Setup(s => s.GetProductEntityByCodeAsync("Prod01"))
                                  .ReturnsAsync(new Product { Code = "Prod01", Name = "Product01", Price = 10, Stock = 100 });

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                              .ReturnsAsync((Order?)null);

            _productServiceMock.Setup(s => s.RemoveStockAsync("Prod01", 2))
                               .ReturnsAsync(Result.Updated);

            _orderRepositoryMock.Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
                               .ReturnsAsync(CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2));

            var expectedDto = new OrderResponseDetailsDto()
            {
                Code = "Order01",
                CustomerCode = "Cust01",
                Status = OrderStatus.InProgress,
                Lines = new List<OrderLineResponseDto>
                {
                    new OrderLineResponseDto { LineNumber = 1, ProductCode = "Prod01", Quantity = 2, Price = 20 }
                },
                TotalPrice = 20
            };

            var result = await _orderService.CreateOrderAsync(new OrderLineRequestCreateDto()
            {
                OrderCode = "Order01",
                ProductCode = "Prod01",
                Quantity = 2
            }, "Cust01");

            result.IsError.Should().BeFalse();
            result.Value.Should().BeEquivalentTo(expectedDto);

            _productServiceMock.Verify(s => s.RemoveStockAsync("Prod01", 2), Times.Once);
            _orderRepositoryMock.Verify(s => s.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_WhenCustomerDoesNotExist_ShouldReturnNotFound()
        {
            _customerServiceMock.Setup(s => s.GetCustomerEntityByCodeAsync("DOESNTEXIST"))
                                   .ReturnsAsync(Error.NotFound("Customer.Code", "A customer with this code doesn't exist."));

            var result = await _orderService.CreateOrderAsync(new OrderLineRequestCreateDto()
            {
                OrderCode = "Order01",
                ProductCode = "Prod01",
                Quantity = 2
            }, "DOESNTEXIST");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task CreateOrder_WhenProductDoesNotExist_ShouldReturnNotFound()
        {
            _customerServiceMock.Setup(s => s.GetCustomerEntityByCodeAsync("Cust01"))
                                   .ReturnsAsync(new Customer { Code = "Cust01", Name = "Customer01", Phone = "123456" });

            _productServiceMock.Setup(s => s.GetProductEntityByCodeAsync("DOESNTEXIST"))
                                  .ReturnsAsync(Error.NotFound("Product.Code", "A product with this code doesn't exist."));

            var result = await _orderService.CreateOrderAsync(new OrderLineRequestCreateDto()
            {
                OrderCode = "Order01",
                ProductCode = "DOESNTEXIST",
                Quantity = 2
            }, "Cust01");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task CreateOrder_WhenOrderExistsAndDifferentCustomer_ShouldReturnValidationError()
        {
            _customerServiceMock.Setup(s => s.GetCustomerEntityByCodeAsync("Cust01"))
                                   .ReturnsAsync(new Customer { Code = "Cust01", Name = "Customer01", Phone = "123456" });

            _productServiceMock.Setup(s => s.GetProductEntityByCodeAsync("Prod01"))
                                  .ReturnsAsync(new Product { Code = "Prod01", Name = "Product01", Price = 10 });

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                              .ReturnsAsync(CreateOrder("Order01", "Cust02", "Customer02", "654321", "Prod01", "Product01", 10, 2));

            var result = await _orderService.CreateOrderAsync(new OrderLineRequestCreateDto()
            {
                OrderCode = "Order01",
                ProductCode = "Prod01",
                Quantity = 2
            }, "Cust01");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task CreateOrder_WhenNotEnoughStock_ShouldReturnValidationError()
        {
            _customerServiceMock.Setup(s => s.GetCustomerEntityByCodeAsync("Cust01"))
                                   .ReturnsAsync(new Customer { Code = "Cust01", Name = "Customer01", Phone = "123456" });

            _productServiceMock.Setup(s => s.GetProductEntityByCodeAsync("Prod01"))
                                  .ReturnsAsync(new Product { Code = "Prod01", Name = "Product01", Price = 10 });

            _orderRepositoryMock.Setup(r => r.GetOrderByCodeAsync("Order01"))
                              .ReturnsAsync(CreateOrder("Order01", "Cust02", "Customer02", "654321", "Prod01", "Product01", 10, 2));

            var result = await _orderService.CreateOrderAsync(new OrderLineRequestCreateDto()
            {
                OrderCode = "Order01",
                ProductCode = "Prod01",
                Quantity = 2
            }, "Cust01");

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
        }
        #endregion

        #region Get all orders
        [Fact]
        public async Task GetAllOrders_WhenOrdersExistInDB_ShouldReturnListOfOrders()
        {
            var orders = new List<Order>
            {
                CreateOrder("Order01", "Cust01", "Customer01", "123456", "Prod01", "Product01", 10, 2),
                CreateOrder("Order02", "Cust02", "Customer02", "654321", "Prod02", "Product02", 15, 3)
            };

            _orderRepositoryMock.Setup(r => r.GetAllOrdersAsync())
                                 .ReturnsAsync(orders);

            var expectedDtos = new List<OrderResponseDto>()
            {
                new OrderResponseDto { Code = "Order01", TotalPrice = 20, CustomerCode = "Cust01", Status = OrderStatus.InProgress },
                new OrderResponseDto { Code = "Order02", TotalPrice = 45, CustomerCode = "Cust02", Status = OrderStatus.InProgress }
            };

            var result = await _orderService.GetAllOrdersAsync();

            result.Should().BeEquivalentTo(expectedDtos);

            _orderRepositoryMock.Verify(r => r.GetAllOrdersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllOrders_WhenNoOrdersExistInDB_ShouldReturnEmptyList()
        {
            _orderRepositoryMock.Setup(r => r.GetAllOrdersAsync())
                                 .ReturnsAsync(new List<Order>());

            var result = await _orderService.GetAllOrdersAsync();

            result.Should().BeEmpty();

            _orderRepositoryMock.Verify(r => r.GetAllOrdersAsync(), Times.Once);
        }
        #endregion

    }
}
