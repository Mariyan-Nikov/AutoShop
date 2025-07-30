namespace AutoShop.Models
{
    // Модел, който представя поръчка в системата
    public class Order
    {
        // Уникален идентификатор на поръчката
        public int Id { get; set; }

        // Идентификатор на клиента, който е направил поръчката
        public int CustomerId { get; set; }

        // Дата и час на създаване на поръчката
        public DateTime OrderDate { get; set; }

        // Статус на поръчката (например Pending, Completed, Cancelled)
        // По подразбиране стойността е "Pending"
        public string Status { get; set; } = "Pending";
    }
}
