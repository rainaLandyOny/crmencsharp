using crmcsharp.Models.entity;
using crmcsharp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class DashboardController : Controller
{
    private readonly ApiClient _apiClient;
    private readonly BudgetService _budgetService;
    private readonly LeadService _leadService;
    private readonly TicketService _ticketService;

    private readonly TicketExpenseService _ticketExpenseService;

    private readonly LeadExpenseService _leadExpenseService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        BudgetService budgetService, 
        TicketExpenseService ticketExpenseService,
        LeadExpenseService leadExpenseService,
        LeadService leadService,
        TicketService ticketService,
        ILogger<DashboardController> logger)
    {
        _budgetService = budgetService;
        _leadService = leadService;
        _ticketService = ticketService;
        _leadExpenseService = leadExpenseService;
        _ticketExpenseService = ticketExpenseService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(DateTime? date1, DateTime? date2)
    {
        try
        {
            date1 ??= DateTime.Now.AddDays(-30);
            date2 ??= DateTime.Now;

            if (date1 > date2)
            {
                TempData["ErrorMessage"] = "La date de début doit être antérieure à la date de fin";
                return RedirectToAction("Index", new { date1 = date2, date2 = date1 });
            }

            var leadsTask = _leadService.GetDateAsync(date1.Value, date2.Value);
            var ticketsTask = _ticketService.GetDateAsync(date1.Value, date2.Value);
            var budgetsTask = _budgetService.GetBudgetsBetweenDatesAsync(date1.Value, date2.Value);
            var totalExpensesLeads = _leadExpenseService.GetDateAsync(date1.Value, date2.Value);
            var totalExpensesTickets = _ticketExpenseService.GetDateAsync(date1.Value,date2.Value);

            await Task.WhenAll(leadsTask, ticketsTask, budgetsTask);

            // Correction des erreurs CS0019 - Conversion explicite des types
            var leads = await leadsTask;
            var tickets = await ticketsTask;
            var budgets = await budgetsTask;

            decimal totalBudget = budgets.Sum(b => b?.Amount ?? 0);
            decimal totalLeadExpense = await totalExpensesLeads;
            decimal totalTicketExpense = await totalExpensesTickets;
            decimal totalExpenses = totalLeadExpense + totalTicketExpense;
            decimal budgetBalance = totalBudget - totalExpenses;

            // Correction des erreurs CS0428 - Ajout des parenthèses pour Count()
            var priorityStats = tickets
                .Where(t => !string.IsNullOrEmpty(t.Priority))
                .GroupBy(t => t.Priority)
                .Select(g => new PriorityStat
                {
                    Priority = g.Key,
                    Count = g.Count(),
                    Percentage = (decimal)g.Count() / tickets.Count() * 100
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            var model = new DashboardViewModel
            {
                StartDate = date1.Value,
                EndDate = date2.Value,
                LeadCount = leads.Count, // Utilisation de la propriété Count au lieu de Count()
                TicketCount = tickets.Count, // Utilisation de la propriété Count au lieu de Count()
                BudgetCount = budgets.Count,
                TotalBudget = totalBudget,
                TotalExpenses = totalExpenses,
                TotalLeadsExpenses = totalLeadExpense,
                TotalTicketExpenses = totalTicketExpense,
                BudgetBalance = budgetBalance,
                PriorityStats = priorityStats,
                RecentLeads = leads.OrderByDescending(l => l.CreatedAt).Take(5).ToList(),
                RecentTickets = tickets.OrderByDescending(t => t.CreatedAt).Take(5).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur dans DashboardController");
            TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement du dashboard";
            return RedirectToAction("Error", "Home");
        }
    }
    public async Task<IActionResult> Details(string type,DateTime? date1, DateTime? date2)
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

                case "leads":
                    var leads = await _leadService.GetDateAsync(date1.Value, date2.Value);;
                    return View("Lead", leads);

                case "tickets":
                    var tickets = await _ticketService.GetDateAsync(date1.Value, date2.Value);
                    return View("Ticket", tickets);

                case "budgets":
                    var budgets = await _budgetService.GetBudgetsBetweenDatesAsync(date1.Value, date2.Value);
                    return View("Budget", budgets);

                default:
                    return RedirectToAction("Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur dans Details");
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Error", "Home");
        }
    }


}

public class DashboardViewModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int LeadCount { get; set; }
    public int TicketCount { get; set; }
    public int BudgetCount { get; set; }
    
    public decimal TotalBudget { get; set; }
    public decimal TotalLeadsExpenses { get; set; }

    public decimal TotalExpenses { get; set; }

    public decimal TotalTicketExpenses { get; set; }
    public decimal BudgetBalance { get; set; }
    
    // Add the missing PriorityStats property
    public List<PriorityStat> PriorityStats { get; set; }
    
    // Données pour les graphiques
    public dynamic ExpenseChartData { get; set; }
    public dynamic BudgetChartData { get; set; }
    public dynamic PriorityChartData { get; set; }
    
    public List<TriggerLeadHisto> RecentLeads { get; set; }
    public List<TicketHisto> RecentTickets { get; set; }
}

// Add this class to represent priority statistics
public class PriorityStat
{
    public string Priority { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
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