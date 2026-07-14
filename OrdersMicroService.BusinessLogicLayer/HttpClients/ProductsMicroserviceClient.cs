using OrdersMicroService.BusinessLogicLayer.DTOs;
using System.Net.Http.Json;

namespace OrdersMicroService.BusinessLogicLayer.HttpClients;
public class ProductsMicroserviceClient
{
    private readonly HttpClient _httpClient;
    public ProductsMicroserviceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductDTO?> GetProductByIdAsync(Guid productId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/{productId}");
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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
        return product;
    }
}
