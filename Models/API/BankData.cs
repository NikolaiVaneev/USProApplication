namespace USProApplication.Models.API
{
    public class BankData
    {
        public string? БИК { get; set; }
        public string? Наим { get; set; }
        public string? НаимАнгл { get; set; }
        public string? Адрес { get; set; }
        public string? Тип { get; set; }
        public BankAccount? КорСчет { get; set; }
    }
}
