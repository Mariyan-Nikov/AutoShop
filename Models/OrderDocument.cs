public class OrderDocument
{
    // Уникален идентификатор на запитването/документа
    public int Id { get; set; }

    // Идентификатор на свързаната кола
    public int CarId { get; set; }

    // Пълно име на клиента, направил запитването
    public string FullName { get; set; } = null!;

    // Телефонен номер за контакт
    public string PhoneNumber { get; set; } = null!;

    // Имейл адрес за контакт
    public string Email { get; set; } = null!;

    // Допълнително съобщение или коментар от клиента (по избор)
    public string? Message { get; set; }

    // Предпочитана дата за среща/действие по запитването
    public DateTime PreferredDate { get; set; }

    // Показва дали запитването е активно (например не е архивирано или изтрито)
    public bool IsActive { get; set; } = true;

    // Дата и час на създаване на запитването, зададена по подразбиране на текущото UTC време
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
