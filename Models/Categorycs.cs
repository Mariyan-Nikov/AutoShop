using System.ComponentModel.DataAnnotations;
namespace AutoShop.Models;
public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Името е задължително")]
    [StringLength(50, ErrorMessage = "Името трябва да е до 50 символа")]
    public string Name { get; set; } = null!;

    [StringLength(200, ErrorMessage = "Описанието трябва да е до 200 символа")]
    public string? Description { get; set; }
}
