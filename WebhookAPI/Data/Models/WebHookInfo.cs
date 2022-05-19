using System.ComponentModel.DataAnnotations;

namespace WebhookAPI.Data.Models;

public class WebHookInfo
{
    [Key] public int Id { get; set; }

    public DateTime Date { get; set; }
    public string Json { get; set; }
}