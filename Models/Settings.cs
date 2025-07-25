namespace AutoShop.Data.Entities
{
    public class Setting
    {
        public int Id { get; set; } // Primary key

        public string Key { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}
