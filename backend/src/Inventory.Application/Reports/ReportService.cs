using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Common.Errors;
using Inventory.Application.Reports.DTOs;

namespace Inventory.Application.Reports;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Result<OverdueOrderReportDto>> GetOverdueOrdersAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        // 1. Validate date range
        if (startDate > endDate)
        {
            return Result<OverdueOrderReportDto>.Failure(
                ReportErrors.InvalidDateRange
            );
        }

        // 2. Convert DateOnly to DateTime
        var fromDate = startDate.ToDateTime(TimeOnly.MinValue);
        var toDate = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        // 3. Get report data
        var items = await _reportRepository.GetOverdueOrdersAsync(
            fromDate,
            toDate,
            cancellationToken
        );

        // 4. Add sequence
        var reportItems = items
            .Select((item, index) =>
                item with { Sequence = index + 1 })
            .ToList();

        // 5. Calculate totals
        var totalAmount = reportItems.Sum(x => x.Amount);
        var totalPrice = reportItems.Sum(x => x.TotalPrice);

        // 6. Return report
        var report = new OverdueOrderReportDto(
            reportItems,
            totalAmount,
            totalPrice
        );

        return Result<OverdueOrderReportDto>.Success(report);
    }
}