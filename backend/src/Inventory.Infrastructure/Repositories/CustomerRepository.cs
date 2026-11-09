using Inventory.Application.Abstractions.Repositories;
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

    public async Task<Customer> AddAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return customer;
    }

    public async Task<bool> DeleteAsync(
        string id,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Customers.FindAsync(
            [id], cancellationToken);

        if (entity is null)
            return false;

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ExistsAsync(
        string id,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AnyAsync(c => c.CusId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CusId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.CusName.Contains(name))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Customers.FindAsync(
            [customer.CusId], cancellationToken);

        if (entity is null)
            return false;

        entity.CusName = customer.CusName;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}