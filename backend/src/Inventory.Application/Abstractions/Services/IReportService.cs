using Inventory.Application.Common;
using Inventory.Application.Reports.DTOs;

namespace Inventory.Application.Abstractions.Services;

public interface IReportService
{
    Task<Result<OverdueOrderReportDto>> GetOverdueOrdersAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );
}
