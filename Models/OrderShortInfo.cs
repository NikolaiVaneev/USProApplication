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
        /// Площадь
        /// </summary>
        public int Square { get; set; }

        /// <summary>
        /// Номер договора
        /// </summary>
        public string ContractNo { get; set; } = string.Empty;

        /// <summary>
        /// Номер счета
        /// </summary>
        public string AccountNo { get; set; } = string.Empty;

        /// <summary>
        /// Дата договора
        /// </summary>
        public DateOnly? ContractDate { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
