namespace BookstoreManagementSystem.Domain.Models
{
    public class Distributor
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
