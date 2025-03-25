using Microsoft.AspNetCore.Mvc;
using crmcsharp.Services;
using crmcsharp.Models.request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace crmcsharp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthRequest authRequest)
        {
            var token = await _accountService.Login(authRequest);

            if (token != null)
            {
                Console.WriteLine("token isn't null");
                HttpContext.Session.SetString("AuthToken", token);
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Effacer le token de session
            HttpContext.Session.Remove("AuthToken");

            // Effacer toutes les données de session
            HttpContext.Session.Clear();

            TempData["Message"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Login", "Account");
        }
    }
}