
using MassTransit;
// using MassTransit.Definition;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Settings;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;



var builder = WebApplication.CreateBuilder(args);


var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();



builder.Services.AddMongo()
    .AddMongoRepository<Item>("items")
    .AddMassTransitWithRabbitMq();


// builder.Services.AddMassTransit(x =>
// {
//     x.UsingRabbitMq((context, configurator) =>
//     {
//         var rabbitMqSettings = builder.Configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
//         configurator.Host(rabbitMqSettings.Host);
//         configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

//     });
// });
// builder.Services.AddHostedService<MassTransitHostedService>();



// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});





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
