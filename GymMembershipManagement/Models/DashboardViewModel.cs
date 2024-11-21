namespace GymMembershipManagement.Models
{
    public class DashboardViewModel
    {
        public decimal MonthlyEarnings { get; set; }
        public decimal YearlyEarnings { get; set; }
        public int TotalActiveMembers { get; set; }
        public int TotalMembers { get; set; }
        public string MonthlyEarningsChartJson { get; set; } 
        public string GenderPieChartJson { get; set; }
        public string AnnualEarningsChartJson { get; set; }
        public string AgeGroupChartJson { get; set; }

        public string MembershipTypeChartJson { get; set; }
        public string StartDateByMonthChartJson { get; set; }

    }
}
