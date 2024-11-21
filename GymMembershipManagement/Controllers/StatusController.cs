using GymMembershipManagement.DataAccess;
using GymMembershipManagement.DataAccess.Models;
using GymMembershipManagement.DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace GymMembershipManagement.Controllers
{

    public class StatusController : Controller
    {
        private readonly MemberService _memberService;
        private readonly ActivityLogService _activityLogService;

        public StatusController(MemberService memberService, ActivityLogService activityLogService)
        {
            _memberService = memberService;
            _activityLogService = activityLogService;
        }


        // Prikaz članova sa statusom
        public IActionResult Index()
        {
            var members = _memberService.GetMembersWithStatus();
            return View(members);
        }

        // Obrada plaćanja
        [HttpPost]
        public IActionResult ProcessPayment(int memberId, string membershipType, decimal amountPaid)
        {
            var success = _memberService.ProcessPayment(memberId, membershipType, amountPaid);

            if (success)
            {
                return Json(new { success = true, message = "Plaćanje uspješno!" });
            }
            else
            {
                return Json(new { success = false, message = "Greška pri plaćanju!" });
            }
        }


        [HttpPost]
        public IActionResult ExtendMembership(int memberId, DateTime newStartDate, string newMembershipType)
        {
            try
            {
               
                _memberService.ExtendMembership(memberId, newStartDate, newMembershipType);

                
                var member = _memberService.GetMemberById(memberId);
                if (member == null)
                {
                    return Json(new { success = false, message = "Član nije pronađen." });
                }

                
                _activityLogService.AddLogAsync(1, "ExtendMembership", $"Admin je produžio članarinu članu: {member.Fullname}");

                return Json(new { success = true, message = "Članarina je uspješno produžena." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Došlo je do greške prilikom produženja članarine." });
            }
        }





    }
}
