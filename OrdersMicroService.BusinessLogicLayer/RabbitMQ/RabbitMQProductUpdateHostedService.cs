using Microsoft.Extensions.Hosting;

namespace OrdersMicroService.BusinessLogicLayer.RabbitMQ;

public class RabbitMQProductUpdateHostedService : IHostedService
{
    private readonly IRabbitMQConsumer _rabbitMQConsumer;
    public RabbitMQProductUpdateHostedService(IRabbitMQConsumer rabbitMQConsumer)
    {
        _rabbitMQConsumer = rabbitMQConsumer;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _rabbitMQConsumer.ConsumeMessageAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}