namespace AutoShop.Data.Entities
{
    public class Setting
    {
        // Основен ключ на настройката
        public int Id { get; set; }

        // Ключ (име) на настройката, например "SiteTitle" или "AdminEmail"
        public string Key { get; set; } = null!;

        // Стойност на настройката като текст
        public string Value { get; set; } = null!;
    }
}
