namespace OrdersMicroService.BusinessLogicLayer.RabbitMQ;
public record ProductDeleteMsg(Guid ProductId, string ProductName);
