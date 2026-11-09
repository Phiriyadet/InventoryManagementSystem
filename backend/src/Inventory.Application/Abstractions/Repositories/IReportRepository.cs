using Inventory.Application.Reports.DTOs;

namespace Inventory.Application.Abstractions.Repositories;

public interface IReportRepository
{
    Task<IReadOnlyList<OverdueOrderReportItemDto>>
        GetOverdueOrdersAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken
        );
}
