namespace USProApplication.Models
{
    public class OrderShortInfo
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Номер договора
        /// </summary>
        public string ContractNo { get; set; } = string.Empty;

        /// <summary>
        /// Дата договора
        /// </summary>
        public DateOnly? ContractDate { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Является основным договором
        /// </summary>
        public bool IsMainOrder { get; set; } = true;

        /// <summary>
        /// Исполнитель
        /// </summary>
        public string? Executor { get; set; }

        /// <summary>
        /// Заказчик
        /// </summary>
        public string Client { get; set; } = string.Empty;

        /// <summary>
        /// Счет
        /// </summary>
        public string Bill { get; set; } = string.Empty;

        /// <summary>
        /// Дополнительные соглашения
        /// </summary>
        public List<OrderShortInfo> AdditionalOrders { get; set; } = [];
    }
}
