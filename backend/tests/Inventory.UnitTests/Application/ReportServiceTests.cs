using FluentAssertions;
using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Common.Errors;
using Inventory.Application.Reports;
using Inventory.Application.Reports.DTOs;
using Moq;

namespace Inventory.UnitTests.Application;

public class ReportServiceTests
{
    private readonly Mock<IReportRepository> _reportRepositoryMock;
    private readonly ReportService _sut;

    public ReportServiceTests()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();
        _sut = new ReportService(_reportRepositoryMock.Object);
    }

    [Fact]
    public async Task GetOverdueOrdersAsync_ShouldReturnFailure_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 10);
        var endDate = new DateOnly(2026, 11, 9);

        // Act
        var result = await _sut.GetOverdueOrdersAsync(
            startDate,
            endDate,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ReportErrors.InvalidDateRange);

        _reportRepositoryMock.Verify(
            x => x.GetOverdueOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task GetOverdueOrdersAsync_ShouldReturnSuccess_WhenNoOrdersExist()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 1);
        var endDate = new DateOnly(2026, 11, 30);

        _reportRepositoryMock
            .Setup(x => x.GetOverdueOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                CancellationToken.None
            ))
            .ReturnsAsync(Array.Empty<OverdueOrderReportItemDto>());

        // Act
        var result = await _sut.GetOverdueOrdersAsync(
            startDate,
            endDate,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalAmount.Should().Be(0);
        result.Value.TotalPrice.Should().Be(0);

        _reportRepositoryMock.Verify(
            x => x.GetOverdueOrdersAsync(
                startDate.ToDateTime(TimeOnly.MinValue),
                endDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetOverdueOrdersAsync_ShouldPassCorrectDateRangeToRepository()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 1);
        var endDate = new DateOnly(2026, 11, 30);

        _reportRepositoryMock
            .Setup(x => x.GetOverdueOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                CancellationToken.None
            ))
            .ReturnsAsync(Array.Empty<OverdueOrderReportItemDto>());

        // Act
        await _sut.GetOverdueOrdersAsync(
            startDate,
            endDate,
            CancellationToken.None
        );

        // Assert
        _reportRepositoryMock.Verify(
            x => x.GetOverdueOrdersAsync(
                startDate.ToDateTime(TimeOnly.MinValue),
                endDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetOverdueOrdersAsync_ShouldAssignSequenceAndCalculateTotals_WhenOrdersExist()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 1);
        var endDate = new DateOnly(2026, 11, 30);

        var items = new List<OverdueOrderReportItemDto>
        {
            new(
                0,
                "C0001",
                "Customer One",
                "G001",
                "Product One",
                new DateTime(2026, 11, 1),
                1,
                new DateTime(2026, 11, 2),
                10,
                100,
                1000
            ),
            new(
                0,
                "C0002",
                "Customer Two",
                "G002",
                "Product Two",
                new DateTime(2026, 11, 3),
                2,
                new DateTime(2026, 11, 4),
                20,
                200,
                4000
            ),
            new(
                0,
                "C0003",
                "Customer Three",
                "G003",
                "Product Three",
                new DateTime(2026, 11, 5),
                3,
                new DateTime(2026, 11, 6),
                30,
                300,
                9000
            )
        };

        _reportRepositoryMock
            .Setup(x => x.GetOverdueOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                CancellationToken.None
            ))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.GetOverdueOrdersAsync(
            startDate,
            endDate,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Items.Should().HaveCount(3);

        result.Value.Items[0].Sequence.Should().Be(1);
        result.Value.Items[1].Sequence.Should().Be(2);
        result.Value.Items[2].Sequence.Should().Be(3);

        result.Value.TotalAmount.Should().Be(60);
        result.Value.TotalPrice.Should().Be(14000);

        _reportRepositoryMock.Verify(
            x => x.GetOverdueOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetOverdueOrdersAsync_ShouldPreserveOrderData_WhenAssigningSequence()
    {
        // Arrange
        var startDate = new DateOnly(2026, 11, 1);
        var endDate = new DateOnly(2026, 11, 30);

        var items = new List<OverdueOrderReportItemDto>
        {
            new(
                0,
                "C0001",
                "Customer One",
                "G001",
                "Product One",
                new DateTime(2026, 11, 1),
                1,
                new DateTime(2026, 11, 2),
                10,
                100,
                1000
            )
        };

        _reportRepositoryMock
            .Setup(x => x.GetOverdueOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                CancellationToken.None
            ))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.GetOverdueOrdersAsync(
            startDate,
            endDate,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();

        var item = result.Value.Items[0];

        item.Sequence.Should().Be(1);
        item.CusId.Should().Be("C0001");
        item.CusName.Should().Be("Customer One");
        item.GoodsId.Should().Be("G001");
        item.GoodsName.Should().Be("Product One");
        item.OrderNo.Should().Be(1);
        item.Amount.Should().Be(10);
        item.UnitCost.Should().Be(100);
        item.TotalPrice.Should().Be(1000);
    }
}
