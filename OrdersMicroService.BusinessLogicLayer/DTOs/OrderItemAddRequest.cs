namespace OrdersMicroService.BusinessLogicLayer.DTOs;
public record OrderItemAddRequest(
    Guid ProductID,
    int Quantity,
    decimal UnitPrice
);
