namespace OrdersMicroService.BusinessLogicLayer.RabbitMQ;
public interface IRabbitMQConsumer
{
    Task ConsumeMessageAsync();
}
