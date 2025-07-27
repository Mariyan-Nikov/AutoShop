public class OrderDocument
{
    public int Id { get; set; }

    public int CarId { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime PreferredDate { get; set; }

    public bool IsActive { get; set; } = true; // Да показва дали запитването е активно

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
