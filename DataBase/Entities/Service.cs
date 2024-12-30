using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace USProApplication.DataBase.Entities
{
    [Table("Services")]
    public class Service : BaseEntity
    {
        [Comment("Наименование")]
        required
        public string Name { get; set; }

        [Comment("Аббревиатура")]
        required
        public string Abbreviation { get; set; } 

        [Comment("Cтоимость")]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Comment("Описание")]
        public string? Description { get; set; }
    }
}
