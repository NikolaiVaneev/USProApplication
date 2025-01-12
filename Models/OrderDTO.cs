using Microsoft.EntityFrameworkCore;

namespace USProApplication.Models
{
    public class OrderDTO
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Number { get; set; }

        public int? Term { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? СompletionDate { get; set; }

        public decimal? Price { get; set; }

        public decimal? PriceToMeter { get; set; }

        public int Square { get; set; } = 0;

        public bool IsCompleted { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public bool UsingNDS { get; set; } = true;
        public sbyte NDS { get; set; } = 5;

        public Guid? ExecutorId { get; set; }

        public Guid? CustomerId { get; set; }

        public ICollection<Guid>? SelectedServicesIds { get; set; }


        public DateTime? PrepaymentBillDate { get; set; }
        public string? PrepaymentBillNumber { get; set; }
        public sbyte? PrepaymentPercent { get; set; } = 0;

        public DateTime? ExecutionBillDate { get; set; }
        public string? ExecutionBillNumber { get; set; }
        public sbyte? ExecutionPercent { get; set; } = 0;

        public DateTime? ApprovalBillDate { get; set; }
        public string? ApprovalBillNumber { get; set; }
        public sbyte? ApprovalPercent { get; set; } = 0;

        public string? AdditionalService { get; set; }
    }
}
