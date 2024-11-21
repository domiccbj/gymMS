namespace GymMembershipManagement.DataAccess.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Level { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
