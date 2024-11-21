using GymMembershipManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace GymMembershipManagement.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            var reports = _reportService.GetMemberPayments();
            return View(reports);
        }
    }
}
