using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMembershipManagement.DataAccess.Models
{
    public class MemberReport
    {
        public int MemberId { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Membership { get; set; }
        public decimal TotalAmount { get; set; }

        public string Picture { get; set; }
    }
}
