namespace USProApplication.Models
{
    public class ClientShortInfo
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
        /// ФИО руководителя
        /// </summary>
        public string ChiefFullName { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Дата заключения договора
        /// </summary>
        public DateTime? ContractDate { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public bool IsExecutor { get; set; }
    }
}