using Microsoft.Extensions.Caching.Distributed;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrdersMicroService.BusinessLogicLayer.HttpClients;
public class ProductsMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _distributedCache;
    public ProductsMicroserviceClient(HttpClient httpClient, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _distributedCache = distributedCache;
    }

    public async Task<ProductDTO?> GetProductByIdAsync(Guid productId)
    {
        // Check if the product is already cached
        string cacheKey = $"Product_{productId}";
        string? cachedProduct = await _distributedCache.GetStringAsync(cacheKey);

        if (cachedProduct != null)
        {
            // If the product is found in the cache, deserialize it and return
            return await Task.FromResult(JsonSerializer.Deserialize<ProductDTO>(cachedProduct));
        }

        HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/products/search/product-id/{productId}");
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                ProductDTO? productFromFallback = await response.Content.ReadFromJsonAsync<ProductDTO>();
                if (productFromFallback == null)
                {
                    throw new HttpRequestException("Product not found or response content is empty.");
                }
                return productFromFallback;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new HttpRequestException($"Bad request: {response.ReasonPhrase}");
            else
                throw new HttpRequestException($"Error retrieving product: {response.ReasonPhrase}");
        }
        ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>();
        if (product == null)
        {
            throw new HttpRequestException("Product not found or response content is empty.");
        }
        // Cache the product for future requests
        string serializedProduct = JsonSerializer.Serialize(product);
        DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2), // Cache for 2 minutes
            SlidingExpiration = TimeSpan.FromMinutes(1) // Reset expiration if accessed within 1 minute
        };

        await _distributedCache.SetStringAsync(cacheKey, serializedProduct, distributedCacheEntryOptions);

        return product;
    }
}
