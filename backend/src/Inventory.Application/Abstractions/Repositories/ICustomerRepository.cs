using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Customer>> GetByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Customer> AddAsync(
        Customer customer,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        Customer customer,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        string id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        string id,
        CancellationToken cancellationToken);
}
