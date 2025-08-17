using System.ComponentModel.DataAnnotations;

namespace HangOut.Domain.Payload.Request.Plan;

public class UpdatePlanRequest
{
    [StringLength(maximumLength: 500, MinimumLength = 1, ErrorMessage = "Tên kế hoạch phải từ 1 đến 500 ký tự.")]
    public string? Name { get; set; }
    [StringLength(maximumLength: 1000, MinimumLength = 1, ErrorMessage = "Vị trí phải từ 1 đến 1000 ký tự.")]
    public string? Location { get; set; }
}