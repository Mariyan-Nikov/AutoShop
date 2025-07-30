namespace AutoShop.Models
{
    public class Review
    {
        // Уникален идентификатор на ревюто
        public int Id { get; set; }

        // Външен ключ към колата, за която е ревюто
        public int CarId { get; set; }

        // Навигационно свойство към съответната кола
        public Car Car { get; set; } = null!;

        // Идентификатор на потребителя, който е написал ревюто
        public string UserId { get; set; } = null!;

        // Текстово съдържание на ревюто
        public string Content { get; set; } = null!;

        // Дата и час на създаване на ревюто
        public DateTime CreatedOn { get; set; }
    }
}
