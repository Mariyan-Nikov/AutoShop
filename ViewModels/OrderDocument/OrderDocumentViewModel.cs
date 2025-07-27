using System;
using System.ComponentModel.DataAnnotations;

namespace AutoShop.ViewModels.OrderDocument
{
    public class OrderDocumentViewModel
    {
        [Required]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Телефонът е задължителен")]
        [Phone(ErrorMessage = "Невалиден телефонен номер")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        public string Email { get; set; } = null!;

        [StringLength(500)]
        public string? Message { get; set; }

        [Required(ErrorMessage = "Моля, изберете дата за среща")]
        [DataType(DataType.Date)]
        public DateTime PreferredDate { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
