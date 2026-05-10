using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrdersMicroService.DataAccessLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<IOrdersRepository, OrdersRepository>();
        return services;
    }
}
