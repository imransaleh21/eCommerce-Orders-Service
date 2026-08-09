using Microsoft.Extensions.Logging;
using OrdersMicroService.BusinessLogicLayer.DTOs;
using System.Net.Http.Json;

namespace eCommerce.Core.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersMicroserviceClient> _logger;

        public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userId}");
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
