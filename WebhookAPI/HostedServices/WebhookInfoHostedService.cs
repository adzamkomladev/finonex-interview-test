using System.Threading.Channels;
using WebhookAPI.Data;
using WebhookAPI.Data.Models;
using WebhookAPI.Dtos;

namespace WebhookAPI.HostedServices;

public class WebhookInfoHostedService : BackgroundService
{
    private readonly Channel<WebHookInfoDto> _channel;
    private readonly ILogger<WebhookInfoHostedService> _logger;
    private readonly IServiceProvider _provider;

    public WebhookInfoHostedService(
        Channel<WebHookInfoDto> channel,
        ILogger<WebhookInfoHostedService> logger,
        IServiceProvider provider)
    {
        _channel = channel;
        _logger = logger;
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!_channel.Reader.Completion.IsCompleted && !stoppingToken.IsCancellationRequested) // if not complete
        {
            // read from channel
            var webhookInfo = await _channel.Reader.ReadAsync(stoppingToken);
            try
            {
                using var scope = _provider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<WebhookDbContext>();

                context.Add(new WebHookInfo
                {
                    Date = webhookInfo.Date,
                    Json = webhookInfo.Json
                });
                await context.SaveChangesAsync(stoppingToken);
                
                _logger.LogInformation("Webhook info saved: {0}", webhookInfo.Json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Webhook info could not be processed!");
            }
        }

        if (!_channel.Reader.Completion.IsCompleted)
        {
            
            var items = _channel.Reader.ReadAllAsync(stoppingToken);
            
            // Write items to file
        }
    }
}