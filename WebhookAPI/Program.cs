using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebhookAPI.Data;
using WebhookAPI.Dtos;
using WebhookAPI.HostedServices;
using WebhookAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<WebhookDbContext>(o => o.UseInMemoryDatabase("webhooks"));

builder.Services.AddSingleton(typeof(Channel<WebHookInfoDto>));
builder.Services.AddHostedService<WebhookInfoHostedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Webhook/Notification API",
        Description = "A .NET Web API for receiving and processing webhooks and notifications effectively"
    });
});

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