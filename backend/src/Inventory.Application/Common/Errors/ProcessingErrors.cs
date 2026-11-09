namespace Inventory.Application.Common.Errors;

public static class ProcessingErrors
{
    public static Error InvalidDateRange =>
        Error.Validation(
            "Processing.InvalidDateRange",
            "The start date must be less than or equal to the end date."
        );
}
