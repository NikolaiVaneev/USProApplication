using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USProApplication.DataBase.Entities;

[Comment("Контрагенты")]
[Table("Counterparties")]
public class Counterparty : BaseEntity
{
    [Comment("Наименование")]
    required
    public string Name { get; set; }

    [Comment("ИНН")]
    [MaxLength(10)]
    public string? INN { get; set; }

    [Comment("ОГРН")]
    [MaxLength(13)]
    public string? OGRN { get; set; }

    [Comment("КПП")]
    [MaxLength(9)]
    public string? KPP { get; set; }

    [Comment("Адрес")]
    public string? Address { get; set; }

    [Comment("Номер счета")]
    public string? PaymentAccount { get; set; }

    [Comment("Банк")]
    public string? Bank { get; set; }

    [Comment("Банковский идентификационный код")]
    [MaxLength(9)]
    public string? BIK { get; set; }

    [Comment("Корреспондентский счет")]
    [MaxLength(20)]
    public string? CorrAccount { get; set; }

    [Comment("Руководитель")]
    public string? Director { get; set; }

    [Comment("Должность руководителя")]
    public DirectorPositions DirectorPosition { get; set; }

    [Comment("Является исполнителем")]
    public bool Executor { get; set; }
}

public enum DirectorPositions
{
    [Description("")]
    None,
    [Description("Директор")]
    Director,
    [Description("Генеральный директор")]
    GeneralDirector,
    [Description("Управляющий")]
    Manager,
    [Description("Начальник")]
    Chief
}
