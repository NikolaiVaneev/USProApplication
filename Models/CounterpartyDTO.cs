using USProApplication.DataBase.Entities;

namespace USProApplication.Models;

public class CounterpartyDTO
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// ИНН
    /// </summary>
    public string? INN { get; set; }

    /// <summary>
    /// ОГРН
    /// </summary>
    public string? OGRN { get; set; }

    /// <summary>
    /// КПП
    /// </summary>
    public string? KPP { get; set; }

    /// <summary>
    /// Адрес
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Номер счета
    /// </summary>
    public string? PaymentAccount { get; set; }

    /// <summary>
    /// Банк
    /// </summary>
    public string? Bank { get; set; }

    /// <summary>
    /// БИК
    /// </summary>
    public string? BIK { get; set; }

    /// <summary>
    /// Корреспондентский счет
    /// </summary>
    public string? CorrAccount { get; set; }

    /// <summary>
    /// Руководитель
    /// </summary>
    public string? Director { get; set; }

    /// <summary>
    /// Должность руководителя
    /// </summary>
    public DirectorPositions DirectorPosition { get; set; }

    /// <summary>
    /// Является исполнителем
    /// </summary>
    public bool Executor { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedOn { get; set; }
}
