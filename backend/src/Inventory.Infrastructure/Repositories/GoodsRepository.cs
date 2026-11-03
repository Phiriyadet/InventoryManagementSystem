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
    public async Task<Goods> AddAsync(CreateGoodsRequest goods, CancellationToken cancellationToken)
    {
        var entity = new Goods
        {
            GoodsId = goods.GoodsId,
            GoodsName = goods.GoodsName,
            CostUnit = goods.CostUnit
        };

        await _context.Goods.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Goods.FindAsync(id, cancellationToken);
        if (entity == null) return false;

        _context.Goods.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _context.Goods.FindAsync(id, cancellationToken);
        
        return result != null;
    }

    public async Task<IEnumerable<Goods>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _context.Goods.ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Goods?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Goods.FindAsync(id, cancellationToken);
        if (entity == null) return null;

        return entity;
    }

    public async Task<IEnumerable<Goods>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var result = await _context.Goods.Where(g => g.GoodsName.Contains(name)).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<Goods?> UpdateAsync(string id, UpdateGoodsRequest goods, CancellationToken cancellationToken)
    {
        var entity = await _context.Goods.FindAsync(id, cancellationToken);
        if (entity == null) return null;

        entity.GoodsName = goods.GoodsName;
        entity.CostUnit = goods.CostUnit;

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

}
