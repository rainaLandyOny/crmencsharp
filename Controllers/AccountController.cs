// using Microsoft.AspNetCore.Mvc;
// using crmcsharp.Services;
// using crmcsharp.Models.request;
// using Microsoft.AspNetCore.Http;

// namespace crmcsharp.Controllers
// {
//     public class AccountController : Controller
//     {
//         private readonly AccountService _accountService;

//         public AccountController(AccountService accountService)
//         {
//             _accountService = accountService;
//         }

//         // GET: Account/Login
//         public IActionResult Login()
//         {
//             return View();
//         }

//         [HttpPost]
//         public async Task<IActionResult> Login(AuthRequest authRequest)
//         {
//             var token = await _accountService.Login(authRequest);

//             if (token != null)
//             {
//                 Console.WriteLine("token isn't null");
//                 HttpContext.Session.SetString("AuthToken", token);
//                 return RedirectToAction("Index", "Home");
//             }
//             else
//             {
//                 ModelState.AddModelError("", "Invalid username or password.");
//                 return View();
//             }
//         }
//     }
// }

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using crmcsharp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AccountService>();

// // Enregistrer le service TicketService
// builder.Services.AddHttpClient<TicketService>(); // Si TicketService utilise HttpClient
// // OU
// builder.Services.AddScoped<TicketService>(); // Si TicketService ne nécessite pas HttpClient
// builder.Services.AddHttpClient<TicketExpenseService>(); // Si TicketService utilise HttpClient
// // OU
// builder.Services.AddScoped<TicketExpenseService>(); // Si TicketService ne nécessite pas HttpClient
// builder.Services.AddScoped<RateConfigService>();
// builder.Services.AddScoped<LeadExpenseService>(); // Si TicketService ne nécessite pas HttpClient
// builder.Services.AddScoped<LeadService>();
// builder.Services.AddSession(options =>
// {
//     options.Cookie.HttpOnly = true; // Sécuriser le cookie
//     options.Cookie.IsEssential = true; // Important pour le fonctionnement de l'app
//     options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cookie envoyé uniquement sur HTTPS
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession(); // Utilisation de la session

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

