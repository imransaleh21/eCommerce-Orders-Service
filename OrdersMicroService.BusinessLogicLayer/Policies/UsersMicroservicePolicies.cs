using Polly;

namespace OrdersMicroService.BusinessLogicLayer.Policies;

public class UsersMicroservicePolicies : IUsersMicroservicePolicies
{
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                      .WaitAndRetryAsync(
                          retryCount: 3,
                          retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      );
    }
}
