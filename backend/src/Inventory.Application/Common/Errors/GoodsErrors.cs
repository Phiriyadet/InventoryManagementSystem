namespace Inventory.Application.Common.Errors;

public static class GoodsErrors
{
    public static Error NotFound(string id) =>
        Error.NotFound("Goods.NotFound", $"No goods were found with the id {id}.");

    public static Error DuplicateId(string id) =>
        Error.Conflict("Goods.DuplicateId", $"A goods with the id {id} already exists.");

    public static Error LengthId =>
        Error.Validation("Goods.LengthId", "The goods id must be 10 characters long.");

    public static Error LengthName =>
        Error.Validation("Goods.LengthName", "The goods name must not exceed 30 characters long.");

    public static readonly Error EmptyId =
        Error.Validation("Goods.EmptyId", "The goods Id cannot be empty.");

    public static readonly Error EmptyName =
        Error.Validation("Goods.EmptyName", "The goods name cannot be empty.");

    public static readonly Error CostPerUnit =
        Error.Validation("Goods.CostPerUnit", "The goods cost per unit must be > 0 and <= 999999.99.");
}
