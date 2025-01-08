using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace USProApplication.DataBase.Entities;

[Comment("Заказы")]
[Table("Orders")]
public class Order : BaseEntity
{
    [Comment("Наименование")]
    public string? Name { get; set; }

    [Comment("Адрес")]
    public string? Address { get; set; }

    [Comment("Номер")]
    public string? Number { get; set; }

    [Comment("Срок выполнения")]
    public int? Term { get; set; }

    [Comment("Дата договора")]
    public DateOnly? StartDate { get; set; }

    [Comment("Дата завершения")]
    public DateOnly? СompletionDate { get; set; }

    [Comment("Цена")]
    public decimal? Price { get; set; }

    [Comment("Цена за метр")]
    public decimal? PriceToMeter { get; set; }

    [Comment("Площадь")]
    public int? Square { get; set; }

    [Comment("Завершено")]
    public bool IsCompleted { get; set; }

    [Comment("Телефон")]
    public string? Phone { get; set; }

    [Comment("Электронная почта")]
    public string? Email { get; set; }

    [Comment("Дата счета предоплаты")]
    public DateOnly? PrepaymentBillDate { get; set; }
    [Comment("Номер счета предоплаты")]
    public string? PrepaymentBillNumber { get; set; }
    [Comment("Процент предоплаты")]
    public int? PrepaymentPercent { get; set; }

    [Comment("Дата счета выполнения")]
    public DateOnly? ExecutionBillDate { get; set; }
    [Comment("Номер счета выполнения")]
    public string? ExecutionBillNumber { get; set; }
    [Comment("Процент оплаты выполнения")]
    public int? ExecutionPercent { get; set; }

    [Comment("Дата счета согласования")]
    public DateOnly? ApprovalBillDate { get; set; }
    [Comment("Номер счета согласования")]
    public string? ApprovalBillNumber { get; set; }
    [Comment("Процент оплаты согласования")]
    public int? ApprovalPercent { get; set; }
    [Comment("НДС")]
    public int? NDS { get; set; }
    [Comment("Идентификатор исполнителя")]
    public Guid? ExecutorId { get; set; }

    /// <summary>
    /// Исполнитель
    /// </summary>
    [ForeignKey(nameof(ExecutorId))]
    public virtual Counterparty? Executor { get; set; }

    [Comment("Идентификатор заказчика")]
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Заказчик
    /// </summary>
    [ForeignKey(nameof(CustomerId))]
    public virtual Counterparty? Customer { get; set; }

    [Comment("Услуги")]
    public virtual ICollection<Service> Services { get; set; } = [];
}
