using System.Text.Json;
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
        if (File.Exists("webhooks.txt"))
        {
            // Read file using StreamReader. Reads file line by line    
            using (var file = new StreamReader("webhooks.txt"))
            {
                string? ln;

                while ((ln = await file.ReadLineAsync()) != null)
                {
                    var webhook = JsonSerializer.Deserialize<WebHookInfoDto>(ln);
                    _channel.Writer.TryWrite(webhook);
                }

                file.Close();
            }

            File.Create("webhooks.txt").Close();
        }

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
            var webhookInfos = _channel.Reader.ReadAllAsync(stoppingToken);

            await using var writer = new StreamWriter("webhooks.txt");
            await foreach (var webhookInfo in webhookInfos)
            {
                var text = JsonSerializer.Serialize(webhookInfo);
                await writer.WriteLineAsync(text);
            }
        }
    }
}