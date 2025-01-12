namespace USProApplication.Models
{
    public interface IDocCreator
    {
        /// <summary>
        /// Создать договор
        /// </summary>
        /// <param name="order">Объект договора</param>
        /// <returns></returns>
        Task CreateContractAsync(OrderDTO order, bool stamp);

        /// <summary>
        /// Создать дополнительный договор
        /// </summary>
        /// <param name="order">Объект договора</param>
        /// <param name="stamp">Штамп</param>
        /// <returns></returns>
        Task CreateAdditionalContractAsync(OrderDTO order, bool stamp);

        /// <summary>
        /// Создать договор-счет
        /// </summary>
        /// <param name="order">Объект договора</param>
        /// <returns></returns>
        Task CreateContractInvoiceAsync(OrderDTO order, bool stamp);

        /// <summary>
        /// Создать акт выполненных работ
        /// </summary>
        /// <param name="order">Объект договора</param>
        /// <returns></returns>
        Task CreateActAsync(OrderDTO order, bool stamp);

        /// <summary>
        /// Создать счет на оплату
        /// </summary>
        /// <param name="order">Объект договора</param>
        /// <param name="type">Тип счета</param>
        /// <returns></returns>
        Task CreatePaymentInvoiceAsync(OrderDTO order, PaymentInvioceTypes type, bool stamp);

        /// <summary>
        /// Создать УПД
        /// </summary>
        /// <param name="order">Объект договора</param>
        /// <returns></returns>
        Task CreateUPDAsync(OrderDTO order, bool stamp);
    }

    /// <summary>
    /// Типы счетов
    /// </summary>
    public enum PaymentInvioceTypes
    {
        /// <summary>
        /// Предоплата
        /// </summary>
        Prepayment,
        /// <summary>
        /// Выполнение
        /// </summary>
        Execution,
        /// <summary>
        /// Согласование
        /// </summary>
        Approval
    }
}
