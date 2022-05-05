using System.ComponentModel.DataAnnotations.Schema;

namespace DayOfFun.Models.View;

[NotMapped]
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}