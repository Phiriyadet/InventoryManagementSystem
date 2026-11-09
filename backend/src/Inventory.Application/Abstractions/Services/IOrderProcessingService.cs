using Inventory.Application.Common;
using Inventory.Application.Processing.DTOs;

namespace Inventory.Application.Abstractions.Services;

public interface IOrderProcessingService
{
    Task<Result<ProcessOrdersResultDto>> ProcessOrdersAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );
}
