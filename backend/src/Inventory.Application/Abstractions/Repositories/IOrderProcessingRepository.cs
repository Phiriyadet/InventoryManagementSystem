namespace Inventory.Application.Abstractions.Repositories;

public interface IOrderProcessingRepository
{
    Task<int> ProcessOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken
    );
}
