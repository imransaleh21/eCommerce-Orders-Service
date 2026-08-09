using Polly;

namespace OrdersMicroService.BusinessLogicLayer.Policies
{
    public interface IOrdersMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();
    }
}
