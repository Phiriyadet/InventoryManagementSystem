using Inventory.Application.Common;
using Inventory.Application.Products.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Services;

public interface IGoodsService
{
    Task<Result<Goods>> GetGoodsByIdAsync(string id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Goods>>> GetGoodsByNameAsync(string name, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Goods>>> GetAllGoodsAsync(CancellationToken cancellationToken);
    Task<Result<Goods>> AddGoodsAsync(CreateGoodsRequest request, CancellationToken cancellationToken);
    Task<Result<Goods>> UpdateGoodsAsync(string id, UpdateGoodsRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteGoodsAsync(string id, CancellationToken cancellationToken);
}
