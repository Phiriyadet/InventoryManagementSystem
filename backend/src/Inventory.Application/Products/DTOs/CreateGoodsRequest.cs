namespace Inventory.Application.Products.DTOs;

public record CreateGoodsRequest
(string GoodsId, string GoodsName, decimal CostUnit);

