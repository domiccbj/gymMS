using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMembershipManagement.DataAccess.Models
{
    public class MemberStatus
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string EmailAddress { get; set; }
        public string Picture { get; set; }
        public string MembershipType { get; set; }
        public DateTime? StartDate { get; set; }
        public int RemainingDays { get; set; }
        public string Status { get; set; } // Active, ExpiringSoon, Expired

        public bool IsNotificationSent { get; set; }
    }

}
