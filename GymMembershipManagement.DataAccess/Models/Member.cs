namespace GymMembershipManagement.DataAccess.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? Sex { get; set; }
        public string? EmailAddress { get; set; } 
        public string? Address { get; set; } 
        public string? Type { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Picture { get; set; } 
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; }
    }

}
