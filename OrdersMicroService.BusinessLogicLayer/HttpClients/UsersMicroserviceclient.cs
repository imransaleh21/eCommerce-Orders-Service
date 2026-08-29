using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace eCommerce.Core.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersMicroserviceClient> _logger;
        private readonly IDistributedCache _distributedCache;

        public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                // Check if the user data is already cached
                string cacheKey = $"User_{userId}";
                string? cacheValue = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    // If the user data is found in the cache, deserialize it and return it
                    return await Task.FromResult(JsonSerializer.Deserialize<UserDTO>(cacheValue));
                }

                HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/users/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return null;

                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        throw new HttpRequestException($"Bad request: {response.ReasonPhrase}");
                    else
                        throw new HttpRequestException($"Error retrieving user: {response.ReasonPhrase}");
                }

                UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();
                if (user == null)
                {
                    throw new HttpRequestException("User not found or response content is empty.");
                }
                // Cache the user data for future requests
                string serializedUser = JsonSerializer.Serialize(user);
                DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4), // Cache for 4 minutes
                    SlidingExpiration = TimeSpan.FromMinutes(2) // Reset expiration if accessed within 2 minutes
                };
                await _distributedCache.SetStringAsync(cacheKey, serializedUser, distributedCacheEntryOptions);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error retrieving user from Users Microservice. Returning dummy user data.");
                return new UserDTO(
                    UserId: userId,
                    Email: "temporarily-unavailable@example.com",
                    PersonName: "User Temporarily Unavailable",
                    Gender: "N/A"
                );
            }
        }
    }
}
