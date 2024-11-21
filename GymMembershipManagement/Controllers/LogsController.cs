using GymMembershipManagement.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace GymMembershipManagement.Controllers
{
    public class LogsController : Controller
    {
        private readonly ActivityLogService _activityLogService;

        public LogsController(ActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _activityLogService.GetLogsAsync();
            return View(logs);
        }
    }
}
