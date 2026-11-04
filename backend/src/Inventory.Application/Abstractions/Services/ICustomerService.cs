using Inventory.Application.Common;
using Inventory.Application.Customers.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Services;

public interface ICustomerService
{
    Task<Result<Customer?>> GetCustomerByIdAsync(string id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Customer>>> GetCustomerByNameAsync(string name, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Customer>>> GetAllCustomerAsync(CancellationToken cancellationToken);
    Task<Result<Customer>> AddCustomerAsync(CreateCustomerRequest customer, CancellationToken cancellationToken);
    Task<Result<Customer?>> UpdateCustomerAsync(string id, UpdateCustomerRequest customer, CancellationToken cancellationToken);
    Task<Result> DeleteCustomerAsync(string id, CancellationToken cancellationToken);
}
