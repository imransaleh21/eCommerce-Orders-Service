namespace OrdersMicroService.BusinessLogicLayer.DTOs;

public record OrderAddRequest(
    Guid UserID,
    DateTime OrderDate,
    List<OrderItemAddRequest> OrderItems
);
