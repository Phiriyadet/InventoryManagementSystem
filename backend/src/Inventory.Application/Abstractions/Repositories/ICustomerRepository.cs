using Inventory.Application.Customers.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IEnumerable<Customer>> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Customer> AddAsync(CreateCustomerRequest customer, CancellationToken cancellationToken);
    Task<Customer?> UpdateAsync(string id, UpdateCustomerRequest customer, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken);
}
