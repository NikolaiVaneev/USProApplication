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

        public int? Square { get; set; }

        public bool IsCompleted { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public Guid? ExecutorId { get; set; }

        public Guid? CustomerId { get; set; }
    }
}
