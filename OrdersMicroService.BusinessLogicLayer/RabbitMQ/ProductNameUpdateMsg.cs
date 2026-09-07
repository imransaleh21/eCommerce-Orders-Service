namespace OrdersMicroService.BusinessLogicLayer.RabbitMQ;
public record ProductNameUpdateMsg(Guid ProductId, string NewProductName);
