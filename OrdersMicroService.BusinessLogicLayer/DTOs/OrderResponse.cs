namespace OrdersMicroService.BusinessLogicLayer.DTOs;

public record OrderResponse(
    Guid OrderID,
    Guid UserID,
    DateTime OrderDate,
    decimal TotalBill,
    List<OrderItemResponse> OrderItems,
    string Email = "",
    string PersonName = ""
    );