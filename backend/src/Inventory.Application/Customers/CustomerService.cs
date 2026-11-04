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
    public async Task<Result<Customer>> AddCustomerAsync(CreateCustomerRequest customer, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customer.CusId))
            return Result<Customer>.Failure(CustomerErrors.EmptyId);
        if (string.IsNullOrWhiteSpace(customer.CusName))
            return Result<Customer>.Failure(CustomerErrors.EmptyName);

        if (customer.CusId.Length < 5 || customer.CusId.Length > 5)
            return Result<Customer>.Failure(CustomerErrors.LengthId);
        if (customer.CusName.Length > 30)
            return Result<Customer>.Failure(CustomerErrors.LengthName);

        var customerExists = await _customerRepository.ExistsAsync(customer.CusId, cancellationToken);
        if (customerExists) return Result<Customer>.Failure(CustomerErrors.DuplicateId(customer.CusId));

        var createdCus = await _customerRepository.AddAsync(
            customer,
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

    public async Task<Result<Customer?>> GetCustomerByIdAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _customerRepository.GetByIdAsync(id, cancellationToken);

        return result != null ? Result<Customer?>.Success(result) : Result<Customer?>.Failure(CustomerErrors.NotFound(id));
    }

    public async Task<Result<IEnumerable<Customer>>> GetCustomerByNameAsync(string name, CancellationToken cancellationToken)
    {
        var list = await _customerRepository.GetByNameAsync(name, cancellationToken);

        return Result<IEnumerable<Customer>>.Success(list);
    }

    public async Task<Result<Customer?>> UpdateCustomerAsync(string id, UpdateCustomerRequest customer, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customer.CusName))
            return Result<Customer?>.Failure(CustomerErrors.EmptyName);

        var result = await _customerRepository.UpdateAsync(id, customer, cancellationToken);

        return result != null ? Result<Customer?>.Success(result) : Result<Customer?>.Failure(CustomerErrors.NotFound(id));
    }
}
