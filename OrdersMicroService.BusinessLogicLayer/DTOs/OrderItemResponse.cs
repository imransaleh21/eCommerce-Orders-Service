namespace OrdersMicroService.BusinessLogicLayer.DTOs;

public record OrderItemResponse(
    Guid ProductID,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string ProductName = "",
    string ProductCategory = ""
);
