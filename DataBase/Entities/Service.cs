#nullable disable

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace USProApplication.DataBase.Entities
{
    [Table("Services")]
    public class Service : BaseEntity
    {
        [Comment("Наименование")]
        public string Name { get; set; }
    }
}
