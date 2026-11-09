using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Common.Errors;
using Inventory.Application.Processing.DTOs;

namespace Inventory.Application.Processing;

public class OrderProcessingService : IOrderProcessingService
{
    private readonly IOrderProcessingRepository _repository;

    public OrderProcessingService(
        IOrderProcessingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProcessOrdersResultDto>> ProcessOrdersAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        // 1. Validate date range
        if (startDate > endDate)
        {
            return Result<ProcessOrdersResultDto>.Failure(
                ProcessingErrors.InvalidDateRange
            );
        }

        // 2. Convert DateOnly to DateTime
        var fromDate = startDate.ToDateTime(TimeOnly.MinValue);
        var toDate = endDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        // 3. Process orders
        var processedCount = await _repository.ProcessOrdersAsync(
            fromDate,
            toDate,
            cancellationToken
        );

        // 4. Return result
        return Result<ProcessOrdersResultDto>.Success(
            new ProcessOrdersResultDto(processedCount)
        );
    }
}
