using crmcsharp.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class DashboardController : Controller
{
    private readonly ApiClient _apiClient;

    public DashboardController()
    {
        _apiClient = new ApiClient("http://localhost:8080");
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new NullableDateTimeConverter() }
            };

            // Récupération des données
            var customers = await _apiClient.GetAsync<List<Customer>>("api/rest/customers", jsonOptions) ?? new List<Customer>();
            var leads = await _apiClient.GetAsync<List<Lead>>("api/rest/leads", jsonOptions) ?? new List<Lead>();
            var tickets = await _apiClient.GetAsync<List<Ticket>>("api/rest/tickets", jsonOptions) ?? new List<Ticket>();
            var budgets = await _apiClient.GetAsync<List<Budget>>("api/rest/budgets", jsonOptions) ?? new List<Budget>();
            var leadExpenses = await _apiClient.GetAsync<List<LeadExpense>>("api/rest/lead-expenses", jsonOptions) ?? new List<LeadExpense>();
            var ticketExpenses = await _apiClient.GetAsync<List<TicketExpense>>("api/rest/ticket-expenses", jsonOptions) ?? new List<TicketExpense>();

            // Calculs des totaux
            decimal totalBudget = budgets.Sum(b => b?.Amount ?? 0);
            decimal totalLeadExpense = leadExpenses.Sum(le => le?.Amount ?? 0);
            decimal totalTicketExpense = ticketExpenses.Sum(te => te?.Amount ?? 0);
            decimal totalExpenses = totalLeadExpense + totalTicketExpense;
            decimal budgetBalance = totalBudget - totalExpenses;

            // Statistiques des tickets par priorité
            var priorityStats = tickets
                .Where(t => t != null)
                .GroupBy(t => t.Priority)
                .Select(g => new 
                {
                    Priority = !string.IsNullOrEmpty(g.Key) ? g.Key : "unknown",
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            // Évolution mensuelle du budget
            var budgetEvolution = budgets
                .Where(b => b?.CreatedAt != null)
                .OrderBy(b => b.CreatedAt)
                .GroupBy(b => b.CreatedAt.Value.ToString("yyyy-MM"))
                .Select(g => new
                {
                    Month = g.Key,
                    Amount = g.Sum(b => b.Amount)
                })
                .ToList();

            // Dépenses par catégorie
            var expensesByCategory = new List<object>
            {
                new { Category = "Lead", Amount = totalLeadExpense },
                new { Category = "Ticket", Amount = totalTicketExpense }
            };

            // Passage des données à la vue
            ViewBag.CustomerCount = customers.Count;
            ViewBag.LeadCount = leads.Count;
            ViewBag.TicketCount = tickets.Count;
            ViewBag.BudgetCount = budgets.Count;
            ViewBag.LeadExpenseCount = leadExpenses.Count;
            ViewBag.TicketExpenseCount = ticketExpenses.Count;
            ViewBag.TotalBudget = totalBudget;
            ViewBag.TotalLeadExpense = totalLeadExpense;
            ViewBag.TotalTicketExpense = totalTicketExpense;
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.BudgetBalance = budgetBalance;
            ViewBag.PriorityStats = priorityStats;
            ViewBag.BudgetEvolution = budgetEvolution;
            ViewBag.ExpensesByCategory = expensesByCategory;

            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur dans DashboardController : {ex.Message}");
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Error", "Home");
        }
    }

    public async Task<IActionResult> Details(string type)
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new NullableDateTimeConverter() }
            };

            switch (type.ToLower())
            {
                case "customers":
                    var customers = await _apiClient.GetAsync<List<Customer>>("api/rest/customers", jsonOptions) ?? new List<Customer>();
                    return View("CustomerDetails", customers);

                case "leads":
                    var leads = await _apiClient.GetAsync<List<Lead>>("api/rest/leads", jsonOptions) ?? new List<Lead>();
                    return View("LeadDetails", leads);

                case "tickets":
                    var tickets = await _apiClient.GetAsync<List<Ticket>>("api/rest/tickets", jsonOptions) ?? new List<Ticket>();
                    return View("TicketDetails", tickets);

                case "budgets":
                    var budgets = await _apiClient.GetAsync<List<Budget>>("api/rest/budgets", jsonOptions) ?? new List<Budget>();
                    return View("BudgetDetails", budgets);

                case "lead-expenses":
                    var leadExpenses = await _apiClient.GetAsync<List<LeadExpense>>("api/rest/lead-expenses", jsonOptions) ?? new List<LeadExpense>();
                    return View("LeadExpenseDetails", leadExpenses);

                case "ticket-expenses":
                    var ticketExpenses = await _apiClient.GetAsync<List<TicketExpense>>("api/rest/ticket-expenses", jsonOptions) ?? new List<TicketExpense>();
                    return View("TicketExpenseDetails", ticketExpenses);

                default:
                    return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur dans Details : {ex.Message}");
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Error", "Home");
        }
    }
}

public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            
            if (reader.TryGetDateTime(out DateTime date))
                return date;
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

