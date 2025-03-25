using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using crmcsharp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AccountService>();

builder.Services.AddHttpClient<BudgetService>(); // Si TicketService utilise HttpClient
// OU
builder.Services.AddScoped<BudgetService>(); 
// Enregistrer le service TicketService
builder.Services.AddHttpClient<TicketService>(); // Si TicketService utilise HttpClient
// OU
builder.Services.AddScoped<TicketService>(); // Si TicketService ne nécessite pas HttpClient
builder.Services.AddHttpClient<TicketExpenseService>(); // Si TicketService utilise HttpClient
// OU
builder.Services.AddScoped<TicketExpenseService>(); // Si TicketService ne nécessite pas HttpClient
builder.Services.AddScoped<RateConfigService>();
builder.Services.AddScoped<LeadExpenseService>(); // Si TicketService ne nécessite pas HttpClient
builder.Services.AddScoped<LeadService>();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // Sécuriser le cookie
    options.Cookie.IsEssential = true; // Important pour le fonctionnement de l'app
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cookie envoyé uniquement sur HTTPS
});

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
