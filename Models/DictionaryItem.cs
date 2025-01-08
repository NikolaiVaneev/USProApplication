namespace USProApplication.Models
{
    /// <summary>
    /// Элемент справочника
    /// </summary>
    public class DictionaryItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}