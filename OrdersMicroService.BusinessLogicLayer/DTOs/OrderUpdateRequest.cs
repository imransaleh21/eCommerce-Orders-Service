namespace OrdersMicroService.BusinessLogicLayer.DTOs;
public record OrderUpdateRequest(
    Guid OrderID,
    Guid UserID,
    DateTime OrderDate,
    List<OrderItemUpdateRequest> OrderItems
);