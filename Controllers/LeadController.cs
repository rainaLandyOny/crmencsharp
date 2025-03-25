using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using crmcsharp.Models.entity;
using crmcsharp.Services;

namespace crmcsharp.Controllers
{
    public class LeadController : Controller
    {
        private readonly LeadService _leadService;
        private readonly LeadExpenseService _leadExpenseService;
        private readonly ILogger<LeadController> _logger;

        public LeadController(LeadService leadService, LeadExpenseService leadExpenseService, ILogger<LeadController> logger)
        {
            _leadService = leadService;
            _leadExpenseService = leadExpenseService;
            _logger = logger;
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var lead = await _leadService.GetLeadByIdAsync(id);

                if (lead == null)
                {
                    TempData["Error"] = "Lead non trouvé.";
                    return RedirectToAction("Index");
                }

                var leadExpenses = await _leadExpenseService.GetByIdAsync(id);

                ViewBag.LatestExpense = leadExpenses;

                return View(lead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des détails du lead.");
                TempData["Error"] = "Erreur lors de la récupération des détails du lead.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Index()
        {
            var leads = await _leadService.GetLeadsAsync();
            return View(leads);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int leadId)
        {
            try
            {
                bool isDeleted = await _leadService.DeleteLead(leadId);

                if (isDeleted)
                {
                    TempData["Message"] = "Lead supprimé avec succès.";
                }
                else
                {
                    TempData["Error"] = "Échec de la suppression du lead.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du lead.");
                TempData["Error"] = "Erreur lors de la suppression du lead.";
            }

            return RedirectToAction("Index");
        }
    }
}
