using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrdersMicroService.DataAccessLayer.Repositories;
using OrdersMicroService.DataAccessLayer.RepositoriesContract;

namespace OrdersMicroService.DataAccessLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbConnectionTemp = configuration.GetConnectionString("MongoDbConnection")!;
        var mongoDbConnection = mongoDbConnectionTemp.Replace("$MONGO_HOST", Environment.GetEnvironmentVariable("MONGO_HOST"))
                                                     .Replace("$MONGO_PORT", Environment.GetEnvironmentVariable("MONGO_PORT"));
        services.AddSingleton<IMongoClient>(new MongoClient(mongoDbConnection));
        services.AddScoped<IMongoDatabase>(provider =>
        {
            IMongoClient client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase("OrdersDataBase");
        });
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        return services;
    }
}
