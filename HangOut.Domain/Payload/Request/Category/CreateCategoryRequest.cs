using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace HangOut.Domain.Payload.Request.Category;

public class CreateCategoryRequest
{
    [Required]
    [StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "Tên danh mục phải từ 1 đến 100 ký tự.")]
    public string Name { get; set; }
    public string? Image { get; set; }

}