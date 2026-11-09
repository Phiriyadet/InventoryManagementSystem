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
    CreateGoodsRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.GoodsId))
            return Result<Goods>.Failure(GoodsErrors.EmptyId);
        if (string.IsNullOrWhiteSpace(request.GoodsName))
            return Result<Goods>.Failure(GoodsErrors.EmptyName);

        if (request.GoodsId.Length != 10)
            return Result<Goods>.Failure(GoodsErrors.LengthId);
        if (request.GoodsName.Length > 30)
            return Result<Goods>.Failure(GoodsErrors.LengthName);

        if (request.CostUnit <= 0 || request.CostUnit > 999_999.99m)
            return Result<Goods>.Failure(GoodsErrors.CostPerUnit);

        var goodsExists = await _goodsRepository.ExistsAsync(request.GoodsId, cancellationToken);
        if (goodsExists) return Result<Goods>.Failure(GoodsErrors.DuplicateId(request.GoodsId));

        var entity = new Goods
        {
            GoodsId = request.GoodsId,
            GoodsName = request.GoodsName,
            CostUnit = request.CostUnit
        };

        var result = await _goodsRepository.AddAsync(
            entity,
            cancellationToken);

        return Result<Goods>.Success(result);
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

    public async Task<Result<Goods>> GetGoodsByIdAsync(string id, CancellationToken cancellationToken)
    {
        var result = await _goodsRepository.GetByIdAsync(id, cancellationToken);

        return result != null ? Result<Goods>.Success(result) : Result<Goods>.Failure(GoodsErrors.NotFound(id));
    }

    public async Task<Result<IEnumerable<Goods>>> GetGoodsByNameAsync(string name, CancellationToken cancellationToken)
    {
        var list = await _goodsRepository.GetByNameAsync(name, cancellationToken);

        return Result<IEnumerable<Goods>>.Success(list);
    }

    public async Task<Result<Goods>> UpdateGoodsAsync(string id, UpdateGoodsRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.GoodsName))
            return Result<Goods>.Failure(GoodsErrors.EmptyName);
        if (request.CostUnit <= 0 || request.CostUnit > 999_999.99m)
            return Result<Goods>.Failure(GoodsErrors.CostPerUnit);

        var entity = new Goods
        {
            GoodsId = id,
            GoodsName = request.GoodsName,
            CostUnit = request.CostUnit
        };

        var result = await _goodsRepository.UpdateAsync(entity, cancellationToken);

        return result ? Result<Goods>.Success(entity) : Result<Goods>.Failure(GoodsErrors.NotFound(id));
    }
}
