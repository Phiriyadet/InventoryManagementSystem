using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Customers.DTOs;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly InventoryDbContext _context;
    public CustomerRepository(InventoryDbContext context)
    {
        _context = context;
    }
    public async Task<Customer> AddAsync(CreateCustomerRequest customer, CancellationToken cancellationToken)
    {
        var entity = new Customer
        {
            CusId = customer.CusId,
            CusName = customer.CusName
        };

        await _context.Customers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Customers.FindAsync(id, cancellationToken);
        if (entity == null) return false;

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _context.Customers.FindAsync(id, cancellationToken);

        return result != null;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _context.Customers.ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Customers.FindAsync(id, cancellationToken);
        if (entity == null) return null;

        return entity;
    }

    public async Task<IEnumerable<Customer>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var result = await _context.Customers.Where(g => g.CusName.Contains(name)).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<Customer?> UpdateAsync(string id, UpdateCustomerRequest customer, CancellationToken cancellationToken)
    {
        var entity = await _context.Customers.FindAsync(id, cancellationToken);
        if (entity == null) return null;

        entity.CusName = customer.CusName;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
