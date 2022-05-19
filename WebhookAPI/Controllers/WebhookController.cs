using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using WebhookAPI.Data;
using WebhookAPI.Dtos;

namespace WebhookAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly WebhookDbContext _context;

    public WebhookController(WebhookDbContext context)
    {
        _context = context;
    }

    [HttpPost("Notifications")]
    public async Task<IActionResult> Notifications(
        [FromServices] Channel<WebHookInfoDto> channel,
        [FromBody] WebHookInfoDto body
    )
    {
        if (!await _context.Database.CanConnectAsync()) return BadRequest("Cannot connect to database");

        await channel.Writer.WriteAsync(body);

        return Ok("Webhook API");
    }
}