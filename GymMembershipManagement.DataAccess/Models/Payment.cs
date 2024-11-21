namespace GymMembershipManagement.DataAccess.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int? Member { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
