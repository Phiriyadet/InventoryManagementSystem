namespace Inventory.Application.Common.Errors;

public static class CustomerErrors
{
    public static Error NotFound(string id) =>
        Error.NotFound("Customers.NotFound", $"No customer were found with the id {id}.");

    public static Error DuplicateId(string id) =>
        Error.Conflict("Customers.DuplicateId", $"A customer with the id {id} already exists.");

    public static Error LengthId =>
    Error.Validation("Customers.LengthId", "The customer id must be 5 characters long.");

    public static Error LengthName =>
        Error.Validation("Customers.LengthName", "The customer name must not exceed 30 characters long.");

    public static readonly Error EmptyId =
        Error.Validation("Customers.EmptyId", "The customer Id cannot be empty.");

    public static readonly Error EmptyName =
        Error.Validation("Customers.EmptyName", "The customer name cannot be empty.");
}
