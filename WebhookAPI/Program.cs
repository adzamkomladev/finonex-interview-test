using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using WebhookAPI.Data;
using WebhookAPI.Dtos;
using WebhookAPI.HostedServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<WebhookDbContext>(o => o.UseInMemoryDatabase("webhooks"));

builder.Services.AddSingleton(Channel.CreateUnbounded<WebHookInfoDto>());
builder.Services.AddHostedService<WebhookInfoHostedService>();

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