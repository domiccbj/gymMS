using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMembershipManagement.DataAccess.Models
{
    public class UserActivityLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string ActionType { get; set; }
        public string ActionDetails { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
