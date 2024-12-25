using Microsoft.EntityFrameworkCore;

namespace USProApplication.DataBase.Entities
{
    public abstract class BaseEntity
    {
        [Comment("Уникальный идентификатор записи.")]
        public Guid Id { get; set; }

        public override string ToString() => Id.ToString();
    }
}
