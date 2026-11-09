namespace Inventory.Application.Common.Errors;

public static class OrderErrors
{
    // Not Found
    public static Error NotFound(int id) =>
        Error.NotFound(
            "Order.NotFound",
            $"No order was found with the order number {id}.");

    public static Error NotFound(int orderNo, string goodsId) =>
        Error.NotFound(
            "OrderDetail.NotFound",
            $"No order detail was found with the order number {orderNo} and goods id {goodsId}.");

    // Validation
    public static readonly Error InvalidOrderDate =
        Error.Validation(
            "Order.InvalidOrderDate",
            "Order creation date is invalid.");

    public static readonly Error InvalidAmount =
        Error.Validation(
            "OrderDetail.InvalidAmount",
            "The order detail amount must be greater than 0 and within the allowed database range.");

    public static readonly Error InvalidOrdDate =
        Error.Validation(
            "OrderDetail.InvalidOrdDate",
            "Order date must not be earlier than the order creation date.");

    public static readonly Error InvalidFinDate =
        Error.Validation("OrderDetail.InvalidFinDate",
            "Finish date must not be earlier than the order date.");
    public static Error AlreadyExists(int orderNo, string goodsId) =>
        Error.Conflict("OrderDetail.AlreadyExists",
        $"Goods '{goodsId}' already exists in Order '{orderNo}'.");
}
