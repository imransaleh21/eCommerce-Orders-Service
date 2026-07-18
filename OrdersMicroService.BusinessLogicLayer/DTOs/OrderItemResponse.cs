namespace OrdersMicroService.BusinessLogicLayer.DTOs;

public record OrderItemResponse(
    Guid ProductID,
    string ProductName,
    string ProductCategory,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
); 
