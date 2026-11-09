using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Products.DTOs;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class GoodsRepository : IGoodsRepository
{
    private readonly InventoryDbContext _context;

    public GoodsRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Goods> AddAsync(
        Goods goods,
        CancellationToken cancellationToken)
    {
        await _context.Goods.AddAsync(goods, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return goods;
    }

    public async Task<bool> DeleteAsync(
        string id,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Goods.FindAsync(
            [id], cancellationToken);

        if (entity is null)
            return false;

        _context.Goods.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ExistsAsync(
        string id,
        CancellationToken cancellationToken)
    {
        return await _context.Goods
            .AnyAsync(g => g.GoodsId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Goods>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Goods
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Goods?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        return await _context.Goods
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GoodsId == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Goods>> GetByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await _context.Goods
            .AsNoTracking()
            .Where(g => g.GoodsName.Contains(name))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(
        Goods goods,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Goods.FindAsync(
            [goods.GoodsId], cancellationToken);

        if (entity is null)
            return false;

        entity.GoodsName = goods.GoodsName;
        entity.CostUnit = goods.CostUnit;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
