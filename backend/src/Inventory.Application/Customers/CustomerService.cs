using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Common.Errors;
using Inventory.Application.Customers.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<Result<Customer>> AddCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CusId))
            return Result<Customer>.Failure(CustomerErrors.EmptyId);
        if (string.IsNullOrWhiteSpace(request.CusName))
            return Result<Customer>.Failure(CustomerErrors.EmptyName);

        if (request.CusId.Length != 5)
            return Result<Customer>.Failure(CustomerErrors.LengthId);
        if (request.CusName.Length > 30)
            return Result<Customer>.Failure(CustomerErrors.LengthName);

        var customerExists = await _customerRepository.ExistsAsync(request.CusId, cancellationToken);
        if (customerExists) return Result<Customer>.Failure(CustomerErrors.DuplicateId(request.CusId));

        var entity = new Customer
        {
            CusId = request.CusId,
            CusName = request.CusName
        };

        var createdCus = await _customerRepository.AddAsync(
            entity,
            cancellationToken);

        return Result<Customer>.Success(createdCus);
    }

    public async Task<Result> DeleteCustomerAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _customerRepository.DeleteAsync(id, cancellationToken);

        return result ? Result.Success() : Result.Failure(CustomerErrors.NotFound(id));
    }

    public async Task<Result<IEnumerable<Customer>>> GetAllCustomerAsync(CancellationToken cancellationToken)
    {
        var list = await _customerRepository.GetAllAsync(cancellationToken);

        return Result<IEnumerable<Customer>>.Success(list);
    }

    public async Task<Result<Customer>> GetCustomerByIdAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _customerRepository.GetByIdAsync(id, cancellationToken);

        return result != null ? Result<Customer>.Success(result) : Result<Customer>.Failure(CustomerErrors.NotFound(id));
    }

    public async Task<Result<IEnumerable<Customer>>> GetCustomerByNameAsync(string name, CancellationToken cancellationToken)
    {
        var list = await _customerRepository.GetByNameAsync(name, cancellationToken);

        return Result<IEnumerable<Customer>>.Success(list);
    }

    public async Task<Result<Customer>> UpdateCustomerAsync(string id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CusName))
            return Result<Customer>.Failure(CustomerErrors.EmptyName);

        var entity = new Customer
        {
            CusId = id,
            CusName = request.CusName
        };

        var result = await _customerRepository.UpdateAsync(entity, cancellationToken);

        return result ? Result<Customer>.Success(entity) : Result<Customer>.Failure(CustomerErrors.NotFound(id));
    }
}
