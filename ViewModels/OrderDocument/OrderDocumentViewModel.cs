// Импортиране на системни библиотеки
using System;
using System.ComponentModel.DataAnnotations;

namespace AutoShop.ViewModels.OrderDocument
{
    // ViewModel за форма за поръчка или резервация на кола (напр. запазване на преглед, тест драйв и др.)
    public class OrderDocumentViewModel
    {
        // Задължително поле – избраната кола, към която е свързан този документ (Id от Car)
        [Required]
        public int CarId { get; set; }

        // Задължително поле – име на клиента
        // Ограничено до максимум 100 символа
        // null! указва, че свойството ще бъде инициализирано от формата, въпреки че компилаторът го счита за потенциално null
        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        // Задължително поле – телефонен номер
        // Изисква се да е валиден телефонен формат според DataAnnotations
        [Required(ErrorMessage = "Телефонът е задължителен")]
        [Phone(ErrorMessage = "Невалиден телефонен номер")]
        public string PhoneNumber { get; set; } = null!;

        // Задължително поле – имейл адрес
        // Валидира се като коректен имейл формат
        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        public string Email { get; set; } = null!;

        // Допълнително съобщение или бележка от клиента
        // Ограничено до 500 символа и може да бъде null
        [StringLength(500)]
        public string? Message { get; set; }

        // Задължително поле – предпочитана дата за среща (например за оглед или тест драйв)
        // Валидира се като дата
        [Required(ErrorMessage = "Моля, изберете дата за среща")]
        [DataType(DataType.Date)]
        public DateTime PreferredDate { get; set; }

        // Дата на създаване на заявката (може да се задава автоматично при създаване)
        public DateTime CreatedOn { get; set; }
    }
}
