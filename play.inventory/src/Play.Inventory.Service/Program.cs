using Play.Common.MassTrasit;
using Play.Common.Middlewares;
using Play.Common.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.ErrorHandler;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(sgr =>
{
    sgr.SwaggerDoc("v1", new() { Title = "Play Inventory Service", Version = "v1" });
});

builder.Services.AddMongoDb()
                .AddMongoRepository<InventoryItem>("inventoryitems")
                .AddMongoRepository<CatalogItem>("CatalogItems")
                .AddMassTransitWithRabbitMQ();


builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CatalogUrl"] ?? "http://localhost:5028");
})
.AddTransientHttpErrorPolicy(builder =>
    builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(5,
     retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                     + TimeSpan.FromMilliseconds((new Random()).Next(0, 1000))
     ))
     .AddTransientHttpErrorPolicy(builder =>
        builder.Or<TimeoutRejectedException>()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(15)

        )
        )
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));




builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false; // Suppress the async suffix in action names

});
var app = builder.Build();
app.UseExceptionHandler(exceptionHanderApp => exceptionHanderApp.ConfigureExceptionHandler());
app.UseMiddleware<RequestTimingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
