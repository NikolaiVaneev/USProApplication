namespace USProApplication.Models
{
    public class ServiceItem : DictionaryItem
    {
        public bool IsChecked { get; set; }
        public decimal Price { get; set; } = 0;
    }
}