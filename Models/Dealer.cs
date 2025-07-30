using System.ComponentModel.DataAnnotations;

namespace AutoShop.Models
{
    public class Dealer
    {
        public int Id { get; set; }
        // Уникален идентификатор на дилъра в базата данни

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100, ErrorMessage = "Името трябва да е до 100 символа")]
        public string Name { get; set; }
        // Име на дилъра (задължително поле, максимум 100 символа)

        [Required(ErrorMessage = "Адресът е задължителен")]
        [StringLength(200, ErrorMessage = "Адресът трябва да е до 200 символа")]
        public string Address { get; set; }
        // Адрес на дилъра (задължително поле, максимум 200 символа)

        [Required(ErrorMessage = "Телефонният номер е задължителен")]
        [Phone(ErrorMessage = "Невалиден телефонен номер")]
        public string PhoneNumber { get; set; }
        // Телефонен номер на дилъра (валиден телефонен формат)

        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        public string Email { get; set; }
        // Имейл адрес на дилъра (валиден имейл формат)
    }
}
