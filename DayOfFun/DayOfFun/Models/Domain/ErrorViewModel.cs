using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

[NotMapped]
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}