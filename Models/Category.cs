using System.ComponentModel.DataAnnotations;

namespace AutoShop.Models;

public class Category
{
    public int Id { get; set; }
    // Уникален идентификатор на категорията в базата данни

    [Required(ErrorMessage = "Името е задължително")]
    [StringLength(50, ErrorMessage = "Името трябва да е до 50 символа")]
    public string Name { get; set; } = null!;
    // Име на категорията (задължително поле, максимум 50 символа)

    [StringLength(200, ErrorMessage = "Описанието трябва да е до 200 символа")]
    public string? Description { get; set; }
    // Описание на категорията (по избор, максимум 200 символа)
}
