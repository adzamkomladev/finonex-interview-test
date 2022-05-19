using System.ComponentModel.DataAnnotations;

namespace WebhookAPI.Dtos;

public class WebHookInfoDto
{
    [Required] public DateTime Date { get; set; }

    [Required] public string Json { get; set; }
}