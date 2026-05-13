namespace OrdersMicroService.BusinessLogicLayer.DTOs;
public record OrderItemUpdateRequest(
    Guid ProductID,
    int Quantity,
    decimal UnitPrice
);