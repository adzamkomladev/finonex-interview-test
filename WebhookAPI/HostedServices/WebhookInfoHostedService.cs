using System.Text.Json;
using WebhookAPI.Data;
using WebhookAPI.Data.Models;
using WebhookAPI.Dtos;
using WebhookAPI.Infrastructure;

namespace WebhookAPI.HostedServices;

public class WebhookInfoHostedService : IHostedService
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
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
                        if (webhook is not null) _channel.Push(webhook);
                    }

                    file.Close();
                }

                File.Create("webhooks.txt").Close();
            }

            while (!_channel.IsComplete() && !cancellationToken.IsCancellationRequested)
            {
                // Uncomment code below to simulate delay in order to show saving into file
                await Task.Delay(1000, cancellationToken).WaitAsync(cancellationToken);

                try
                {
                    using var scope = _provider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<WebhookDbContext>();

                    if (!await context.Database.CanConnectAsync(cancellationToken))
                        throw new Exception("Cannot connect to database");

                    var webhookInfo = await _channel.Read();

                    if (webhookInfo is null) throw new Exception("Invalid webhook info");

                    context.Add(new WebHookInfo
                    {
                        Date = webhookInfo.Date,
                        Json = webhookInfo.Json
                    });
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Webhook info could not be processed!");
                }
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_channel.IsComplete())
        {
            var webhookInfos = await _channel.ReadAll();

            await using var writer = new StreamWriter("webhooks.txt");
            foreach (var webhookInfo in webhookInfos)
            {
                var text = JsonSerializer.Serialize(webhookInfo);
                await writer.WriteLineAsync(text);
            }
        }
    }
}