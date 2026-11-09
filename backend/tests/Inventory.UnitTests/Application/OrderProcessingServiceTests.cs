using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Common.Errors;
using Inventory.Application.Processing;
using Moq;

namespace Inventory.UnitTests.Application;


public class OrderProcessingServiceTests
{
    private readonly Mock<IOrderProcessingRepository> _repositoryMock;
    private readonly OrderProcessingService _sut;

    public OrderProcessingServiceTests()
    {
        _repositoryMock = new Mock<IOrderProcessingRepository>();
        _sut = new OrderProcessingService(_repositoryMock.Object);
    }

    [Fact]
    public async Task ProcessOrdersAsync_ValidDateRange_ReturnsSuccess()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 1);
        var endDate = new DateOnly(2026, 11, 9);
        var cancellationToken = CancellationToken.None;
        const int processedCount = 5;

        _repositoryMock
            .Setup(x => x.ProcessOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                cancellationToken))
            .ReturnsAsync(processedCount);

        // Act
        var result = await _sut.ProcessOrdersAsync(
            startDate,
            endDate,
            cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(processedCount, result.Value.ProcessedCount);

        _repositoryMock.Verify(x => x.ProcessOrdersAsync(
            startDate.ToDateTime(TimeOnly.MinValue),
            endDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ProcessOrdersAsync_StartDateAfterEndDate_ReturnsFailure()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 10);
        var endDate = new DateOnly(2026, 11, 9);

        // Act
        var result = await _sut.ProcessOrdersAsync(
            startDate,
            endDate,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ProcessingErrors.InvalidDateRange, result.Error);

        _repositoryMock.Verify(x => x.ProcessOrdersAsync(
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessOrdersAsync_SameStartAndEndDate_ReturnsSuccess()
    {
        // Arrange
        var date = new DateOnly(2026, 11, 9);

        _repositoryMock
            .Setup(x => x.ProcessOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        // Act
        var result = await _sut.ProcessOrdersAsync(
            date,
            date,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.ProcessedCount);

        _repositoryMock.Verify(x => x.ProcessOrdersAsync(
            date.ToDateTime(TimeOnly.MinValue),
            date.AddDays(1).ToDateTime(TimeOnly.MinValue),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessOrdersAsync_NoOrdersProcessed_ReturnsSuccessWithZeroCount()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 1);
        var endDate = new DateOnly(2026, 11, 9);

        _repositoryMock
            .Setup(x => x.ProcessOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.ProcessOrdersAsync(
            startDate,
            endDate,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.ProcessedCount);
    }

    [Fact]
    public async Task ProcessOrdersAsync_ValidDateRange_PassesCancellationTokenToRepository()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 1);
        var endDate = new DateOnly(2026, 11, 9);
        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        _repositoryMock
            .Setup(x => x.ProcessOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                cancellationToken))
            .ReturnsAsync(1);

        // Act
        await _sut.ProcessOrdersAsync(
            startDate,
            endDate,
            cancellationToken);

        // Assert
        _repositoryMock.Verify(x => x.ProcessOrdersAsync(
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            cancellationToken), Times.Once);
    }
}
