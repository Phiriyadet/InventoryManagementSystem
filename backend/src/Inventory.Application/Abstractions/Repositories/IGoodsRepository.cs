using Inventory.Application.Products.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Repositories;

public interface IGoodsRepository
{
    Task<Goods?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Goods>> GetByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Goods>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Goods> AddAsync(
        Goods goods,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        Goods goods,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        string id,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        string id,
        CancellationToken cancellationToken);
}
