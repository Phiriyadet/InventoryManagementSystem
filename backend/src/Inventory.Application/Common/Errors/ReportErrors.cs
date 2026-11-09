namespace Inventory.Application.Common.Errors;

public static class ReportErrors
{
    public static Error InvalidDateRange =>
        Error.Validation(
            "Report.InvalidDateRange",
            "The start date must be less than or equal to the end date."
        );
}
