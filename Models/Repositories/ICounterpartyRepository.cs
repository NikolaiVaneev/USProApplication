namespace USProApplication.Models.Repositories
{
    public interface ICounterpartyRepository : IBaseRepository<CounterpartyDTO>
    {
        Task<ICollection<ClientShortInfo>> GetCounterpartiesShortInfos();

        /// <summary>
        /// Проверка существования контрагента
        /// </summary>
        /// <param name="INN">ИНН</param>
        /// <returns>Название организации</returns>
        Task<string?> CheckCounterpartyExistAsync(string? INN);
    }
}