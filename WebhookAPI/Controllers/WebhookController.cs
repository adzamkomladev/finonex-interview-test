using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using WebhookAPI.Dtos;

namespace WebhookAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    [HttpPost("Notifications")]
    public async Task<IActionResult> Notifications(
        [FromServices] Channel<WebHookInfoDto> channel,
        [FromBody] WebHookInfoDto body
    )
    {
        Console.WriteLine("Webhook received {0}, {1}", body.Date, body.Json);

        await channel.Writer.WriteAsync(body);

        return Ok("Webhook API");
    }
}