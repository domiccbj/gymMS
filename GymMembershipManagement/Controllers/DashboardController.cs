using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GymMembershipManagement.DataAccess;
using GymMembershipManagement.Models;
using Newtonsoft.Json;

namespace GymMembershipManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            if (HttpContext.Session.GetString("Username") == null)
            {
                context.Result = RedirectToAction("Login", "Account");
            }

            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                MonthlyEarnings = _dashboardService.CalculateMonthlyEarnings(),
                YearlyEarnings = _dashboardService.CalculateYearlyEarnings(),
                TotalActiveMembers = _dashboardService.CountTotalActiveMembers(),
                TotalMembers = _dashboardService.CountTotalMembers(),
                MonthlyEarningsChartJson = _dashboardService.GetMonthlyEarningsChartData(),
                GenderPieChartJson = _dashboardService.GetGenderPieChartData(),
                AnnualEarningsChartJson = _dashboardService.GetAnnualEarningsChartData(), 
                AgeGroupChartJson = _dashboardService.GetAgeGroupChartData(),
                MembershipTypeChartJson = _dashboardService.GetMembershipTypeChartData(),
                StartDateByMonthChartJson = _dashboardService.GetStartDateByMonthChartData()
            };

            return View(viewModel);
        }
    }

}

