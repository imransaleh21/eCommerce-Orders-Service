using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.QualityOfService.Polly;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("Ocelot.json",
    optional: false, 
    reloadOnChange: true);
builder.Services
    .AddOcelot()
    .AddPolly();
var app = builder.Build();

await app.UseOcelot();
app.Run();
