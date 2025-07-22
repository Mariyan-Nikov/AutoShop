namespace AutoShop.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
