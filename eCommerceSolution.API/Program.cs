using eCommerce.Core.HttpClients;
using eCommerceSolution.API.MiddleWares;
using OrdersMicroService.BusinessLogicLayer;
using OrdersMicroService.BusinessLogicLayer.HttpClients;
using OrdersMicroService.BusinessLogicLayer.Policies;
using OrdersMicroService.DataAccessLayer;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBusinessLogicLayer(builder.Configuration);
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure HttpClient for microservices
// Polly is used to add resilience and transient fault handling capabilities to the HttpClient.
builder.Services.AddHttpClient<UsersMicroserviceClient>(
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiGateway:BaseUrl"] ?? throw new InvalidOperationException("ApiGateway:BaseUrl is not configured."));
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }).AddPolicyHandler((serviceProvider, request) =>
        serviceProvider.GetRequiredService<IUsersMicroservicePolicies>().GetRetryPolicy())
     .AddPolicyHandler((serviceProvider, request) =>
        serviceProvider.GetRequiredService<IUsersMicroservicePolicies>().GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<ProductsMicroserviceClient>(
    client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiGateway:BaseUrl"] ?? throw new InvalidOperationException("ApiGateway:BaseUrl is not configured."));
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }).AddPolicyHandler((serviceProvider, request) =>
        serviceProvider.GetRequiredService<IOrdersMicroservicePolicies>().GetFallbackPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandlingMiddleware();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
