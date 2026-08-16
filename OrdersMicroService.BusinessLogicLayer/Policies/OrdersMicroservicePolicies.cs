using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using Polly;

namespace OrdersMicroService.BusinessLogicLayer.Policies;

public class OrdersMicroservicePolicies : IOrdersMicroservicePolicies
{
    private readonly ILogger<OrdersMicroservicePolicies> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _fallbackPolicy;

    public OrdersMicroservicePolicies(ILogger<OrdersMicroservicePolicies> logger)
    {
        _logger = logger;

        _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
            .Or<Exception>()
            .FallbackAsync(
                fallbackAction: async (cancellationToken) =>
                {
                    _logger.LogInformation("Fallback policy executed due to microservice call failure.");

                    ProductDTO dummyProduct = new ProductDTO(
                        ProductID: Guid.Empty,
                        ProductName: "Temporarily Unavailable",
                        ProductCategory: "N/A",
                        UnitPrice: 0,
                        QuantityInStock: 0
                    );

                    var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(
                            JsonSerializer.Serialize(dummyProduct),
                            Encoding.UTF8,
                            "application/json"
                        )
                    };

                    return responseMessage;
                },
                onFallbackAsync: async (outcome) =>
                {
                    _logger.LogInformation("Fallback policy triggered due to: {ReasonPhrase}", outcome.Result?.ReasonPhrase);
                }
            );
    }

    public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy() => _fallbackPolicy;
}

