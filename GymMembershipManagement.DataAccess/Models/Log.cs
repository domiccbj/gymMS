namespace GymMembershipManagement.DataAccess.Models
{
    public class Log
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Logs { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
