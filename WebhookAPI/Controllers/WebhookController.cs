using Microsoft.AspNetCore.Mvc;
using WebhookAPI.Data;
using WebhookAPI.Data.Models;
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
    public async Task<IActionResult> Notifications([FromBody] WebHookInfoDto body)
    {
        Console.WriteLine("Webhook received {0}, {1}", body.Date, body.Json);

        _context.Add(new WebHookInfo()
        {
            Date = body.Date,
            Json = body.Json
        });

        await _context.SaveChangesAsync();

        return Ok("Webhook API");
    }
}