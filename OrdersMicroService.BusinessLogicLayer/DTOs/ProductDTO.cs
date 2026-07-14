namespace OrdersMicroService.BusinessLogicLayer.DTOs;

public record ProductDTO(
Guid ProductID,
string ProductName,
string ProductCategory,
double? UnitPrice,
int? QuantityInStock
);
