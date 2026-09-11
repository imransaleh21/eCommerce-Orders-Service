using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrdersMicroService.BusinessLogicLayer.RabbitMQ;
public class RabbitMQConsumer : IRabbitMQConsumer, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly ILogger<RabbitMQConsumer> _logger;

    public RabbitMQConsumer(IConfiguration configuration, ILogger<RabbitMQConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RABBITMQ_HOST"]!,
            Port = int.Parse(_configuration["RABBITMQ_PORT"]!),
            UserName = _configuration["RABBITMQ_USER"]!,
            Password = _configuration["RABBITMQ_PASS"]!
        };

        _connection = connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public async Task ConsumeMessageAsync()
    {
        const string updateRouteKey = "product.name.updated";
        const string deleteRouteKey = "product.deleted";
        string queueName = _configuration["RABBITMQ_ORDERS_PRODUCTS_QUEUE"]!;
        // Create queue if it doesn't exist
        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            autoDelete: false,
            exclusive: false
        );
        // Create exchange if it doesn't exist (but cxchange can be created at the project startup)
        string exchangeName = _configuration["RABBITMQ_PRODUCTS_EXCHANGE"]!;
        await _channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false
        );
        // Bind queue to exchange with the routing key
        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: updateRouteKey
        );
        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: deleteRouteKey
        );

        // Create a consumer to listen for messages
        var consumer = new AsyncEventingBasicConsumer(_channel);
        // Handle received messages
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                switch (eventArgs.RoutingKey)
                {
                    case updateRouteKey:
                        var productUpdateMessage = JsonSerializer.Deserialize<ProductNameUpdateMsg>(message);
                        if (productUpdateMessage is not null)
                        {
                            _logger.LogInformation($"Received product update: " + $"ProductID={productUpdateMessage.ProductId}, " + $"NewName={productUpdateMessage.NewProductName}");
                            // Business logic goes here
                        }
                        break;
                    case deleteRouteKey:
                        var productDeleteMessage = JsonSerializer.Deserialize<ProductDeleteMsg>(message);
                        if (productDeleteMessage is not null)
                        {
                            _logger.LogInformation($"Received product delete: " + $"ProductID={productDeleteMessage.ProductId}, " + $"ProductName={productDeleteMessage.ProductName}");
                            // Business logic goes here
                        }
                        break;
                    default:
                        _logger.LogWarning($"Received message with unknown routing key: {eventArgs.RoutingKey}");
                        break;
                }
                // Acknowledge successful processing
                await _channel.BasicAckAsync( eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError( $"Error processing message: {ex.Message}");
                // Later:
                // Nack / retry / dead-letter queue
            }
        };

        // Start consuming messages
        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer
        );
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
