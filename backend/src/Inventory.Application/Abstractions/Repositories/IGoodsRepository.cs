using Inventory.Application.Products.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Repositories;

public interface IGoodsRepository
{
    Task<Goods?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IEnumerable<Goods>> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Goods>> GetAllAsync(CancellationToken cancellationToken);
    Task<Goods> AddAsync(CreateGoodsRequest goods, CancellationToken cancellationToken);
    Task<Goods?> UpdateAsync(string id, UpdateGoodsRequest goods, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken);
}
