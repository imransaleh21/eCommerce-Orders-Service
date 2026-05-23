using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersMicroService.BusinessLogicLayer.Mappers;
using FluentValidation;
using OrdersMicroService.BusinessLogicLayer.Validators;

namespace OrdersMicroService.BusinessLogicLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<IOrdersService, OrdersService>();
        services.AddAutoMapper(cfg => { }, typeof(OrderAddRequestToOrderMappingProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
        return services;
    }
}
