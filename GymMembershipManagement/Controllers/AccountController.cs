using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GymMembershipManagement.Models;
using GymMembershipManagement.DataAccess;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace GymMembershipManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly ActivityLogService _activityLogService;

        public AccountController(UserRepository userRepository, ActivityLogService activityLogService)
        {
            _userRepository = userRepository;
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Provjera sesije - ako je korisnik već prijavljen
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user != null)
            {
                // Provjeri hash lozinke
                if (PasswordHasher.VerifyPassword(password, user.Password))
                {
                    HttpContext.Session.SetString("Username", user.Username);

                    // Dodaj log za login
                    await _activityLogService.AddLogAsync(user.Id, "Login", $"{user.Username} se prijavio");

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Pogrešan username ili password!";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Pogrešan username ili password!";
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Dohvati korisničko ime iz sesije
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
            {
                // Pretpostavljamo da korisničko ime postoji u bazi
                var user = _userRepository.GetUserByUsername(username);

                if (user != null)
                {
                    // Dodaj log za logout
                    await _activityLogService.AddLogAsync(user.Id, "Logout", $"{user.Username} se odjavio");
                }
            }

            // Očisti sesiju
            HttpContext.Session.Clear();

            // Redirekt na login stranicu
            return RedirectToAction("Login");
        }




        public static class PasswordHasher
        {
            public static string HashPassword(string password)
            {
                using (var sha256 = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(password);
                    var hash = sha256.ComputeHash(bytes);
                    return Convert.ToBase64String(hash);
                }
            }

            public static bool VerifyPassword(string password, string hashedPassword)
            {
                var hashOfInput = HashPassword(password);
                return hashOfInput == hashedPassword;
            }
        }


    }


}
