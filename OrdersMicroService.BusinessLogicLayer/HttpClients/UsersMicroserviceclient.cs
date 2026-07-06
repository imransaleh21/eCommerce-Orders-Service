using OrdersMicroService.BusinessLogicLayer.DTOs;
using System.Net.Http.Json;

namespace eCommerce.Core.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        public UsersMicroserviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
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
    }
}
