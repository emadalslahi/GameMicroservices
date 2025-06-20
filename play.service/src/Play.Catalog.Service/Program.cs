using MassTransit;
using Play.Catalog.Entities;
using Play.Catalog.Service.ErrorHandler;
using Play.Common.MassTrasit;
using Play.Common.Middlewares;
using Play.Common.Repositories;
using Play.Common.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(sgr =>
{
    sgr.SwaggerDoc("v1", new() { Title = "Play Catalog Service", Version = "v1" });
});


builder.Services.AddMongoDb()
      .AddMongoRepository<Item>("Items"); // Register MongoDB configuration and dependency injection

builder.Services.AddMassTransitWithRabbitMQ();
// builder.Services.AddMassTransitHostedService(); // No longer needed in recent MassTransit versions

 
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false; // Suppress the async suffix in action names

});

var app = builder.Build();
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.ConfigureExceptionHandler()); // Configure the exception handler
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
