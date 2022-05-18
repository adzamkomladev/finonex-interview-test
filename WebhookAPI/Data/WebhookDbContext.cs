using Microsoft.EntityFrameworkCore;
using WebhookAPI.Data.Models;

namespace WebhookAPI.Data;

public class WebhookDbContext : DbContext
{
    public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options)
    {
    }

    public DbSet<WebHookInfo> WebHookInfos { get; set; }
}