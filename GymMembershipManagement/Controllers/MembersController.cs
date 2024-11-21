using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GymMembershipManagement.DataAccess.Models;
using GymMembershipManagement.DataAccess;
using GymMembershipManagement.DataAccess.Services;

namespace GymMembershipManagement.Controllers
{
    public class MembersController : Controller
    {
        private readonly MemberService _memberService;
        private readonly ActivityLogService _activityLogService;

        public MembersController(MemberService memberService, ActivityLogService activityLogService)
        {
            _memberService = memberService;
            _activityLogService = activityLogService;
        }
        

        public IActionResult Index()
        {
            var members = _memberService.GetAllMembers();
            return View(members);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Member member, IFormFile? picture)
        {
            if (ModelState.IsValid)
            {
                if (picture != null)
                {
                    var uploadDirectory = Path.Combine("wwwroot", "images", "slikeclanova");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(picture.FileName);
                    var filePath = Path.Combine(uploadDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        picture.CopyTo(stream);
                    }

                    member.Picture = Path.Combine("/images/slikeclanova", fileName).Replace("\\", "/");
                }

                // Poziv metode koja dodaje člana i automatski unosi u tablicu Payments
                _memberService.AddMember(member);

               

                await _activityLogService.AddLogAsync(1, "Create", $"Admin je kreirao novog člana: {member.Fullname}");

                return RedirectToAction("Index");
            }

            return View(member);
        }


        [HttpGet]
        public JsonResult Edit(int id)
        {
            var member = _memberService.GetMemberById(id);
            if (member == null)
            {
                return Json(new { success = false, message = "Član nije pronađen!" });
            }

            var sanitizedMember = new
            {
                member.Id,
                Fullname = member.Fullname ?? "",
                Phone = member.Phone ?? "",
                EmailAddress = member.EmailAddress ?? "",
                Address = member.Address ?? "",
                Sex = member.Sex ?? "",
                Type = member.Type ?? "",
                Birthdate = member.Birthdate?.ToString("yyyy-MM-dd"), 
                StartDate = member.StartDate?.ToString("yyyy-MM-dd"), 
                Picture = member.Picture ?? "",
                Description = member.Description ?? ""
            };

            return Json(new { success = true, data = sanitizedMember });
        }




        [HttpPost]
        public async Task<JsonResult> Edit(Member member, IFormFile? picture)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Podaci nisu ispravni!" });
            }

            try
            {
                
                if (picture != null)
                {
                    var uploadDirectory = Path.Combine("wwwroot", "images", "slikeclanova");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(picture.FileName);
                    var filePath = Path.Combine(uploadDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        picture.CopyTo(stream);
                    }

                    member.Picture = Path.Combine("/images/slikeclanova", fileName).Replace("\\", "/");
                }
                else
                {
                    
                    var existingMember = _memberService.GetMemberById(member.Id);
                    if (existingMember != null)
                    {
                        member.Picture = existingMember.Picture;
                    }
                }

                
                _memberService.UpdateMember(member);

                

                await _activityLogService.AddLogAsync(1,"Edit",$"Admin je ažurirao člana: {member.Fullname}"
        );

                return Json(new { success = true, message = "Član uspješno ažuriran!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Greška pri ažuriranju člana: " + ex.Message });
            }
        }



        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                // Dohvati podatke o članu prije brisanja za logiranje
                var member = _memberService.GetMemberById(id);
                if (member == null)
                {
                    return Json(new { success = false, message = "Član nije pronađen!" });
                }

                // Provjeri postoji li slika i obriši je s diska
                if (!string.IsNullOrEmpty(member.Picture))
                {
                    var picturePath = Path.Combine("wwwroot", member.Picture.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(picturePath))
                    {
                        System.IO.File.Delete(picturePath);
                    }
                }

                // Obriši člana iz baze
                _memberService.DeleteMember(id);

                // Dodaj log za brisanje člana
                await _activityLogService.AddLogAsync(
                    1,
                    "Delete",
                    $"Admin je obrisao člana: {member.Fullname}"
                );

                return Json(new { success = true, message = "Član uspješno obrisan!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Greška pri brisanju člana: " + ex.Message });
            }
        }


    }

}
