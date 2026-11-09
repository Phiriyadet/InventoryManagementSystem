using FluentAssertions;
using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Orders;
using Inventory.Application.Orders.DTOs;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IGoodsRepository> _goodsRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly OrderService _sut;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private static readonly DateTime OrderDay = new(2026, 11, 9);

    public OrderServiceTests()
    {
        _sut = new OrderService(
            _orderRepository.Object,
            _goodsRepository.Object,
            _customerRepository.Object);
    }

    #region Request Builders

    // หมายเหตุ: ส่งวันที่ว่างให้ใช้ default(DateTime) (ไม่ใช่ null)
    // เพราะ null จะถูกแทนด้วยค่า default ของ builder

    private static CreateHOrderRequest BuildCreateHRequest(
        string cusId = "C001",
        DateTime? orderDate = null)
        => new(cusId, orderDate ?? OrderDay);

    private static CreateDOrderRequest BuildCreateDRequest(
        int orderNo = 1,
        string goodsId = "G001",
        DateTime? ordDate = null,
        DateTime? finDate = null,
        decimal amount = 10m)
        => new(orderNo, goodsId, ordDate ?? OrderDay, finDate, amount);

    private static UpdateDOrderRequest BuildUpdateDRequest(
        string goodsId = "G001",
        DateTime? ordDate = null,
        DateTime? finDate = null,
        decimal amount = 10m)
        => new(goodsId, ordDate ?? OrderDay, finDate, amount);

    #endregion

    #region CreateOrderAsync

    [Fact]
    public async Task CreateOrderAsync_ShouldReturnFailure_WhenOrderDateIsDefault()
    {
        var request = BuildCreateHRequest(orderDate: default(DateTime));

        var result = await _sut.CreateOrderAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _customerRepository.Verify(
            x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldReturnFailure_WhenCustomerDoesNotExist()
    {
        var request = BuildCreateHRequest();

        _customerRepository
            .Setup(x => x.ExistsAsync(request.CusId, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _sut.CreateOrderAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _orderRepository.Verify(
            x => x.AddOrderAsync(It.IsAny<OrderHeader>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        var request = BuildCreateHRequest();

        var expected = new OrderHeader
        {
            OrderNo = 1,
            OrderDate = request.OrderDate,
            CusId = request.CusId
        };

        _customerRepository
            .Setup(x => x.ExistsAsync(request.CusId, _cancellationToken))
            .ReturnsAsync(true);

        _orderRepository
            .Setup(x => x.AddOrderAsync(
                It.IsAny<OrderHeader>(), _cancellationToken))
            .ReturnsAsync(expected);

        var result = await _sut.CreateOrderAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(
            x => x.AddOrderAsync(
                It.Is<OrderHeader>(o =>
                    o.CusId == request.CusId &&
                    o.OrderDate == request.OrderDate),
                _cancellationToken),
            Times.Once);
    }

    #endregion

    #region AddOrderDetailAsync

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(10000000)]
    public async Task AddOrderDetailAsync_ShouldReturnFailure_WhenAmountIsInvalid(
        decimal amount)
    {
        var request = BuildCreateDRequest(amount: amount);

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _orderRepository.Verify(
            x => x.GetOrderByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldReturnFailure_WhenOrdDateIsDefault()
    {
        var request = BuildCreateDRequest(ordDate: default(DateTime));

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldReturnFailure_WhenOrderDoesNotExist()
    {
        var request = BuildCreateDRequest();

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync((OrderHeader?)null);

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _goodsRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldReturnFailure_WhenOrdDateIsBeforeOrderDate()
    {
        var request = BuildCreateDRequest(ordDate: new DateTime(2026, 11, 8));

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync(new OrderHeader
            {
                OrderNo = 1,
                OrderDate = OrderDay
            });

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _goodsRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldReturnFailure_WhenFinDateIsBeforeOrdDate()
    {
        var request = BuildCreateDRequest(
            ordDate: OrderDay,
            finDate: new DateTime(2026, 11, 8));

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync(new OrderHeader
            {
                OrderNo = 1,
                OrderDate = new DateTime(2026, 11, 1)
            });

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _goodsRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldReturnFailure_WhenGoodsDoesNotExist()
    {
        var request = BuildCreateDRequest();

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync(new OrderHeader
            {
                OrderNo = 1,
                OrderDate = new DateTime(2026, 11, 1)
            });

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G001", _cancellationToken))
            .ReturnsAsync((Goods?)null);

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _orderRepository.Verify(
            x => x.AddOrderDetailAsync(It.IsAny<OrderDetail>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldCalculateTotalPrice_WhenRequestIsValid()
    {
        var request = BuildCreateDRequest(
            finDate: new DateTime(2026, 11, 15),
            amount: 10m);

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync(new OrderHeader
            {
                OrderNo = 1,
                OrderDate = new DateTime(2026, 11, 1)
            });

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G001", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G001", CostUnit = 150 });

        _orderRepository
            .Setup(x => x.AddOrderDetailAsync(
                It.IsAny<OrderDetail>(), _cancellationToken))
            .ReturnsAsync((OrderDetail detail, CancellationToken _) => detail);

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(
            x => x.AddOrderDetailAsync(
                It.Is<OrderDetail>(d =>
                    d.OrderNo == 1 &&
                    d.GoodsId == "G001" &&
                    d.Amount == 10 &&
                    d.CostUnit == 150 &&
                    d.TotPrc == 1500),
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task AddOrderDetailAsync_ShouldUseDefaultFinDate_WhenFinDateIsNull()
    {
        var request = BuildCreateDRequest(finDate: null, amount: 5m);

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync(new OrderHeader
            {
                OrderNo = 1,
                OrderDate = new DateTime(2026, 11, 1)
            });

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G001", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G001", CostUnit = 100 });

        _orderRepository
            .Setup(x => x.AddOrderDetailAsync(
                It.IsAny<OrderDetail>(), _cancellationToken))
            .ReturnsAsync((OrderDetail detail, CancellationToken _) => detail);

        var result = await _sut.AddOrderDetailAsync(request, _cancellationToken);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(
            x => x.AddOrderDetailAsync(
                It.Is<OrderDetail>(d =>
                    d.FinDate == new DateTime(2000, 1, 1)),
                _cancellationToken),
            Times.Once);
    }

    #endregion

    #region UpdateOrderDetailAsync

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10000000)]
    public async Task UpdateOrderDetailAsync_ShouldReturnFailure_WhenAmountIsInvalid(
        decimal amount)
    {
        var request = BuildUpdateDRequest(amount: amount);

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _goodsRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldReturnFailure_WhenNewGoodsDoesNotExist()
    {
        var request = BuildUpdateDRequest(goodsId: "G002");

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G002", _cancellationToken))
            .ReturnsAsync((Goods?)null);

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _orderRepository.Verify(
            x => x.GetOrderDetailByIdAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldReturnFailure_WhenOriginalDetailDoesNotExist()
    {
        var request = BuildUpdateDRequest();

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G001", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G001", CostUnit = 100 });

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync((OrderDetail?)null);

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldReturnFailure_WhenNewGoodsAlreadyExists()
    {
        var request = BuildUpdateDRequest(goodsId: "G002");

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G002", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G002", CostUnit = 200 });

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(new OrderDetail { OrderNo = 1, GoodsId = "G001" });

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G002", _cancellationToken))
            .ReturnsAsync(new OrderDetail { OrderNo = 1, GoodsId = "G002" });

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _orderRepository.Verify(
            x => x.ReplaceOrderDetailAsync(
                It.IsAny<OrderDetail>(),
                It.IsAny<OrderDetail>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldReplaceDetail_WhenGoodsIdChanges()
    {
        var request = BuildUpdateDRequest(
            goodsId: "G002",
            finDate: new DateTime(2026, 11, 15),
            amount: 5m);

        var original = new OrderDetail
        {
            OrderNo = 1,
            GoodsId = "G001",
            Amount = 2
        };

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G002", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G002", CostUnit = 200 });

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(original);

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G002", _cancellationToken))
            .ReturnsAsync((OrderDetail?)null);

        _orderRepository
            .Setup(x => x.ReplaceOrderDetailAsync(
                It.IsAny<OrderDetail>(),
                It.IsAny<OrderDetail>(),
                _cancellationToken))
            .ReturnsAsync(true);

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(
            x => x.ReplaceOrderDetailAsync(
                original,
                It.Is<OrderDetail>(d =>
                    d.OrderNo == 1 &&
                    d.GoodsId == "G002" &&
                    d.Amount == 5 &&
                    d.CostUnit == 200 &&
                    d.TotPrc == 1000),
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldReturnFailure_WhenReplaceFails()
    {
        var request = BuildUpdateDRequest(goodsId: "G002", amount: 5m);

        var original = new OrderDetail
        {
            OrderNo = 1,
            GoodsId = "G001"
        };

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G002", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G002", CostUnit = 100 });

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(original);

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G002", _cancellationToken))
            .ReturnsAsync((OrderDetail?)null);

        _orderRepository
            .Setup(x => x.ReplaceOrderDetailAsync(
                It.IsAny<OrderDetail>(),
                It.IsAny<OrderDetail>(),
                _cancellationToken))
            .ReturnsAsync(false);

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldUpdateDetail_WhenGoodsIdDoesNotChange()
    {
        var request = BuildUpdateDRequest(
            finDate: new DateTime(2026, 11, 15),
            amount: 10m);

        var original = new OrderDetail
        {
            OrderNo = 1,
            GoodsId = "G001",
            Amount = 2,
            CostUnit = 50,
            TotPrc = 100
        };

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G001", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G001", CostUnit = 150 });

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(original);

        _orderRepository
            .Setup(x => x.UpdateOrderDetailAsync(
                It.IsAny<OrderDetail>(), _cancellationToken))
            .ReturnsAsync(true);

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(
            x => x.UpdateOrderDetailAsync(
                It.Is<OrderDetail>(d =>
                    d.Amount == 10 &&
                    d.CostUnit == 150 &&
                    d.TotPrc == 1500),
                _cancellationToken),
            Times.Once);

        _orderRepository.Verify(
            x => x.ReplaceOrderDetailAsync(
                It.IsAny<OrderDetail>(),
                It.IsAny<OrderDetail>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateOrderDetailAsync_ShouldReturnFailure_WhenUpdateFails()
    {
        var request = BuildUpdateDRequest();

        _goodsRepository
            .Setup(x => x.GetByIdAsync("G001", _cancellationToken))
            .ReturnsAsync(new Goods { GoodsId = "G001", CostUnit = 100 });

        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(new OrderDetail { OrderNo = 1, GoodsId = "G001" });

        _orderRepository
            .Setup(x => x.UpdateOrderDetailAsync(
                It.IsAny<OrderDetail>(), _cancellationToken))
            .ReturnsAsync(false);

        var result = await _sut.UpdateOrderDetailAsync(
            1, "G001", request, _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region DeleteOrderAsync

    [Fact]
    public async Task DeleteOrderAsync_ShouldReturnSuccess_WhenOrderIsDeleted()
    {
        _orderRepository
            .Setup(x => x.DeleteOrderAsync(1, _cancellationToken))
            .ReturnsAsync(true);

        var result = await _sut.DeleteOrderAsync(1, _cancellationToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteOrderAsync_ShouldReturnFailure_WhenOrderDoesNotExist()
    {
        _orderRepository
            .Setup(x => x.DeleteOrderAsync(1, _cancellationToken))
            .ReturnsAsync(false);

        var result = await _sut.DeleteOrderAsync(1, _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region DeleteOrderDetailAsync

    [Fact]
    public async Task DeleteOrderDetailAsync_ShouldReturnSuccess_WhenDetailIsDeleted()
    {
        _orderRepository
            .Setup(x => x.DeleteOrderDetailAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(true);

        var result = await _sut.DeleteOrderDetailAsync(
            1, "G001", _cancellationToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteOrderDetailAsync_ShouldReturnFailure_WhenDetailDoesNotExist()
    {
        _orderRepository
            .Setup(x => x.DeleteOrderDetailAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(false);

        var result = await _sut.DeleteOrderDetailAsync(
            1, "G001", _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region GetOrderByIdAsync

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnSuccess_WhenOrderExists()
    {
        var expected = new OrderHeader
        {
            OrderNo = 1,
            CusId = "C001"
        };

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync(expected);

        var result = await _sut.GetOrderByIdAsync(1, _cancellationToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnFailure_WhenOrderDoesNotExist()
    {
        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync((OrderHeader?)null);

        var result = await _sut.GetOrderByIdAsync(1, _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region GetOrderDetailByIdAsync

    [Fact]
    public async Task GetOrderDetailByIdAsync_ShouldReturnSuccess_WhenDetailExists()
    {
        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync(new OrderDetail
            {
                OrderNo = 1,
                GoodsId = "G001"
            });

        var result = await _sut.GetOrderDetailByIdAsync(
            1, "G001", _cancellationToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetOrderDetailByIdAsync_ShouldReturnFailure_WhenDetailDoesNotExist()
    {
        _orderRepository
            .Setup(x => x.GetOrderDetailByIdAsync(1, "G001", _cancellationToken))
            .ReturnsAsync((OrderDetail?)null);

        var result = await _sut.GetOrderDetailByIdAsync(
            1, "G001", _cancellationToken);

        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region GetOrderDetailsAsync

    [Fact]
    public async Task GetOrderDetailsAsync_ShouldReturnFailure_WhenOrderDoesNotExist()
    {
        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync((OrderHeader?)null);

        var result = await _sut.GetOrderDetailsAsync(1, _cancellationToken);

        result.IsSuccess.Should().BeFalse();

        _orderRepository.Verify(
            x => x.GetOrderDetailsAsync(
                It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetOrderDetailsAsync_ShouldReturnSuccess_WhenOrderExists()
    {
        var details = new List<OrderDetail>
        {
            new() { OrderNo = 1, GoodsId = "G001" },
            new() { OrderNo = 1, GoodsId = "G002" }
        };

        _orderRepository
            .Setup(x => x.GetOrderByIdAsync(1, _cancellationToken))
            .ReturnsAsync(new OrderHeader { OrderNo = 1 });

        _orderRepository
            .Setup(x => x.GetOrderDetailsAsync(1, _cancellationToken))
            .ReturnsAsync(details);

        var result = await _sut.GetOrderDetailsAsync(1, _cancellationToken);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(
            x => x.GetOrderDetailsAsync(1, _cancellationToken),
            Times.Once);
    }

    #endregion

    #region GetOrderListAsync

    [Fact]
    public async Task GetOrderListAsync_ShouldReturnSuccess()
    {
        var expected = new List<OrderSummaryDto>
        {
            new("C001", "Customer One", 1, 2, 1500m)
        };

        _orderRepository
            .Setup(x => x.GetOrderListAsync(_cancellationToken))
            .ReturnsAsync(expected);

        var result = await _sut.GetOrderListAsync(_cancellationToken);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(
            x => x.GetOrderListAsync(_cancellationToken),
            Times.Once);
    }

    #endregion
}