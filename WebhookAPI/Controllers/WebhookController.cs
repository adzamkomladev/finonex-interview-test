using Microsoft.AspNetCore.Mvc;
using WebhookAPI.Data;
using WebhookAPI.Dtos;
using WebhookAPI.Infrastructure;

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

    /// <summary>
    ///     Receives webhook/notification information
    /// </summary>
    /// <returns>An acknowledgement of receiving a webhook info data</returns>
    /// <remarks>
    ///     Sample request:
    ///     POST /Notifications
    ///     {
    ///     "date": "2022-05-19T12:11:14.673Z",
    ///     "json":
    ///     "{\"id\":\"1\",\"name\":\"test\",\"description\":\"test\",\"status\":\"test\",\"created_at\":\"2022-05-19T12:11:14.673Z\",\"updated_at\":\"2022-05-19T12:11:14.673Z\"}"
    ///     }
    /// </remarks>
    /// <response code="200">Returns message that webhook has received data</response>
    /// <response code="400">If the db doesn't exist</response>
    [HttpPost("Notifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Notifications(
        [FromServices] Channel<WebHookInfoDto> channel,
        [FromBody] WebHookInfoDto body
    )
    {
        if (!await _context.Database.CanConnectAsync()) return BadRequest("Cannot connect to database");

        channel.Push(body);

        return Ok("Webhook has received data");
    }
}