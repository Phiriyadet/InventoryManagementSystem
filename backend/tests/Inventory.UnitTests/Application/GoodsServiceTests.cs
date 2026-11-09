using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Products;
using Inventory.Application.Products.DTOs;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application;

public class GoodsServiceTests
{
    private readonly Mock<IGoodsRepository> _goodsRepositoryMock;
    private readonly GoodsService _sut;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GoodsServiceTests()
    {
        _goodsRepositoryMock = new Mock<IGoodsRepository>();
        _sut = new GoodsService(_goodsRepositoryMock.Object);
    }

    #region Request Builders

    private static CreateGoodsRequest BuildCreateRequest(
        string goodsId = "GOODS00001",
        string goodsName = "Test Goods",
        decimal costUnit = 100m)
        => new(goodsId, goodsName, costUnit);

    private static UpdateGoodsRequest BuildUpdateRequest(
        string goodsName = "Updated Goods",
        decimal costUnit = 200m)
        => new(goodsName, costUnit);

    #endregion

    #region AddGoodsAsync

    [Fact]
    public async Task AddGoodsAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        var request = BuildCreateRequest();

        _goodsRepositoryMock
            .Setup(x => x.ExistsAsync(request.GoodsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _goodsRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Goods>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Goods goods, CancellationToken _) => goods);

        var result = await _sut.AddGoodsAsync(request, _cancellationToken);

        Assert.True(result.IsSuccess);
        _goodsRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Goods>(g =>
                    g.GoodsId == request.GoodsId &&
                    g.GoodsName == request.GoodsName &&
                    g.CostUnit == request.CostUnit),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task AddGoodsAsync_ShouldFail_WhenGoodsIdIsEmpty(string goodsId)
    {
        var request = BuildCreateRequest(goodsId: goodsId);

        var result = await _sut.AddGoodsAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        _goodsRepositoryMock.Verify(
            x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task AddGoodsAsync_ShouldFail_WhenGoodsNameIsEmpty(string goodsName)
    {
        var request = BuildCreateRequest(goodsName: goodsName);

        var result = await _sut.AddGoodsAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        _goodsRepositoryMock.Verify(
            x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("12345678901")]
    public async Task AddGoodsAsync_ShouldFail_WhenGoodsIdLengthIsInvalid(string goodsId)
    {
        var request = BuildCreateRequest(goodsId: goodsId);

        var result = await _sut.AddGoodsAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task AddGoodsAsync_ShouldFail_WhenGoodsNameExceedsMaximumLength()
    {
        var request = BuildCreateRequest(goodsName: new string('A', 31));

        var result = await _sut.AddGoodsAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000000)]
    public async Task AddGoodsAsync_ShouldFail_WhenCostUnitIsInvalid(decimal costUnit)
    {
        var request = BuildCreateRequest(costUnit: costUnit);

        var result = await _sut.AddGoodsAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task AddGoodsAsync_ShouldFail_WhenGoodsIdAlreadyExists()
    {
        var request = BuildCreateRequest();

        _goodsRepositoryMock
            .Setup(x => x.ExistsAsync(request.GoodsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.AddGoodsAsync(request, _cancellationToken);

        Assert.False(result.IsSuccess);
        _goodsRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Goods>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion

    #region DeleteGoodsAsync

    [Fact]
    public async Task DeleteGoodsAsync_ShouldReturnSuccess_WhenGoodsExists()
    {
        _goodsRepositoryMock
            .Setup(x => x.DeleteAsync("GOODS00001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteGoodsAsync("GOODS00001", _cancellationToken);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteGoodsAsync_ShouldFail_WhenGoodsDoesNotExist()
    {
        _goodsRepositoryMock
            .Setup(x => x.DeleteAsync("GOODS00001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.DeleteGoodsAsync("GOODS00001", _cancellationToken);

        Assert.False(result.IsSuccess);
    }

    #endregion

    #region GetAllGoodsAsync

    [Fact]
    public async Task GetAllGoodsAsync_ShouldReturnAllGoods()
    {
        var goods = new List<Goods>
        {
            new() { GoodsId = "GOODS00001", GoodsName = "Goods 1", CostUnit = 100m },
            new() { GoodsId = "GOODS00002", GoodsName = "Goods 2", CostUnit = 200m }
        };

        _goodsRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(goods);

        var result = await _sut.GetAllGoodsAsync(_cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    #endregion

    #region GetGoodsByIdAsync

    [Fact]
    public async Task GetGoodsByIdAsync_ShouldReturnGoods_WhenGoodsExists()
    {
        var goods = new Goods
        {
            GoodsId = "GOODS00001",
            GoodsName = "Test Goods",
            CostUnit = 100m
        };

        _goodsRepositoryMock
            .Setup(x => x.GetByIdAsync(goods.GoodsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goods);

        var result = await _sut.GetGoodsByIdAsync(goods.GoodsId, _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(goods, result.Value);
    }

    [Fact]
    public async Task GetGoodsByIdAsync_ShouldFail_WhenGoodsDoesNotExist()
    {
        _goodsRepositoryMock
            .Setup(x => x.GetByIdAsync("GOODS00001", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Goods?)null);

        var result = await _sut.GetGoodsByIdAsync("GOODS00001", _cancellationToken);

        Assert.False(result.IsSuccess);
    }

    #endregion

    #region GetGoodsByNameAsync

    [Fact]
    public async Task GetGoodsByNameAsync_ShouldReturnMatchingGoods()
    {
        var goods = new List<Goods>
        {
            new() { GoodsId = "GOODS00001", GoodsName = "Test Goods", CostUnit = 100m }
        };

        _goodsRepositoryMock
            .Setup(x => x.GetByNameAsync("Test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(goods);

        var result = await _sut.GetGoodsByNameAsync("Test", _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetGoodsByNameAsync_ShouldReturnEmptyList_WhenNoGoodsMatch()
    {
        _goodsRepositoryMock
            .Setup(x => x.GetByNameAsync("Unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Goods>());

        var result = await _sut.GetGoodsByNameAsync("Unknown", _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    #endregion

    #region UpdateGoodsAsync

    [Fact]
    public async Task UpdateGoodsAsync_ShouldReturnSuccess_WhenGoodsExists()
    {
        var request = BuildUpdateRequest();

        _goodsRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Goods>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateGoodsAsync(
            "GOODS00001",
            request,
            _cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("GOODS00001", result.Value.GoodsId);
        Assert.Equal(request.GoodsName, result.Value.GoodsName);
        Assert.Equal(request.CostUnit, result.Value.CostUnit);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateGoodsAsync_ShouldFail_WhenGoodsNameIsEmpty(string goodsName)
    {
        var request = BuildUpdateRequest(goodsName: goodsName);

        var result = await _sut.UpdateGoodsAsync(
            "GOODS00001",
            request,
            _cancellationToken);

        Assert.False(result.IsSuccess);
        _goodsRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Goods>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1_000_000)]
    public async Task UpdateGoodsAsync_ShouldFail_WhenCostUnitIsInvalid(decimal costUnit)
    {
        var request = BuildUpdateRequest(costUnit: costUnit);

        var result = await _sut.UpdateGoodsAsync(
            "GOODS00001",
            request,
            _cancellationToken);

        Assert.False(result.IsSuccess);
        _goodsRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Goods>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateGoodsAsync_ShouldFail_WhenGoodsDoesNotExist()
    {
        var request = BuildUpdateRequest();

        _goodsRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Goods>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.UpdateGoodsAsync(
            "GOODS00001",
            request,
            _cancellationToken);

        Assert.False(result.IsSuccess);
    }

    #endregion
}