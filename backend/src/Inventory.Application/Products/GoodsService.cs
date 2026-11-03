using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Abstractions.Services;
using Inventory.Application.Common;
using Inventory.Application.Common.Errors;
using Inventory.Application.Products.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Products;

public class GoodsService : IGoodsService
{
    private readonly IGoodsRepository _goodsRepository;
    public GoodsService(IGoodsRepository goodsRepository)
    {
        _goodsRepository = goodsRepository;
    }
    public async Task<Result<Goods>> AddGoodsAsync(
    CreateGoodsRequest goods,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(goods.GoodsId))
            return Result<Goods>.Failure(GoodsErrors.EmptyId);
        if (string.IsNullOrWhiteSpace(goods.GoodsName))
            return Result<Goods>.Failure(GoodsErrors.EmptyName);

        if (goods.GoodsId.Length < 10 || goods.GoodsId.Length > 10)
            return Result<Goods>.Failure(GoodsErrors.LengthId);
        if (goods.GoodsName.Length > 30)
            return Result<Goods>.Failure(GoodsErrors.LengthName);

        if (goods.CostUnit <= 0 || goods.CostUnit > 999999.99m)
            return Result<Goods>.Failure(GoodsErrors.CostPerUnit);

        var goodsExists = await _goodsRepository.ExistsAsync(goods.GoodsId, cancellationToken);
        if (goodsExists) return Result<Goods>.Failure(GoodsErrors.DuplicateId(goods.GoodsId));

        var createdGoods = await _goodsRepository.AddAsync(
            goods,
            cancellationToken);

        return Result<Goods>.Success(createdGoods);
    }

    public async Task<Result> DeleteGoodsAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _goodsRepository.DeleteAsync(id, cancellationToken);

        return result ? Result.Success() : Result.Failure(GoodsErrors.NotFound(id));

    }

    public async Task<Result<IEnumerable<Goods>>> GetAllGoodsAsync(CancellationToken cancellationToken)
    {
        var list = await _goodsRepository.GetAllAsync(cancellationToken);

        return Result<IEnumerable<Goods>>.Success(list);
    }

    public async Task<Result<Goods?>> GetGoodsByIdAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _goodsRepository.GetByIdAsync(id, cancellationToken);

        return result != null ? Result<Goods?>.Success(result) : Result<Goods?>.Failure(GoodsErrors.NotFound(id));
    }

    public async Task<Result<IEnumerable<Goods>>> GetGoodsByNameAsync(string name, CancellationToken cancellationToken)
    {
        var list = await _goodsRepository.GetByNameAsync(name, cancellationToken);

        return Result<IEnumerable<Goods>>.Success(list);
    }

    public async Task<Result<Goods?>> UpdateGoodsAsync(string id, UpdateGoodsRequest goods, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(goods.GoodsName))
            return Result<Goods?>.Failure(GoodsErrors.EmptyName);
        if (goods.CostUnit <= 0 || goods.CostUnit > 999999.99m)
            return Result<Goods?>.Failure(GoodsErrors.CostPerUnit);

        var result = await _goodsRepository.UpdateAsync(id, goods, cancellationToken);

        return result != null ? Result<Goods?>.Success(result) : Result<Goods?>.Failure(GoodsErrors.NotFound(id));
    }
}
