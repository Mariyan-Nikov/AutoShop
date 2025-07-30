namespace AutoShop.Models
{
    // Модел за предаване на информация при грешки в приложението
    public class ErrorViewModel
    {
        // Идентификатор на текущата заявка, който помага при проследяване на грешки и логване
        public string? RequestId { get; set; }

        // Връща true, ако RequestId не е null или празен — това служи за показване на RequestId във view-то
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
