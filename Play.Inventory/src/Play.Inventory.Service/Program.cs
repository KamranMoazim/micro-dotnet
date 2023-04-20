using MassTransit;
using MongoDB.Driver.Encryption;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var serviceSettings = new ServiceSettings();

var builder = WebApplication.CreateBuilder(args);



serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryItem")
    .AddMongoRepository<CatalogItem>("catalogItem")
    .AddMassTransitWithRabbitMq();



// builder.Services.AddSingleton<CatalogClient>();




builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5000");
})
.AddTransientHttpErrorPolicy(build => build.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    3,
    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(100),
    onRetry: (exception, retryCount, context) =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?
            .LogWarning($"Delaying for {retryCount.TotalSeconds}. Retrying call to catalog service. Attempt {retryCount}");
    }
))
.AddTransientHttpErrorPolicy(build => build.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    3,
    TimeSpan.FromSeconds(15),
    onBreak: (outcome, timespan) =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?
            .LogWarning($"Opeing CircuitBreaker for {timespan.TotalSeconds}.");
    },
    onReset: () =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?
            .LogWarning($"Closing CircuitBreaker.");
    }
))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
