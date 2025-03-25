using Microsoft.AspNetCore.Mvc;
using crmcsharp.Services;
using crmcsharp.Models.entity;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace crmcsharp.Controllers
{
    public class LeadExpenseController : Controller
    {
        private readonly LeadExpenseService _leadExpenseService;
        private readonly ILogger<LeadExpenseController> _logger;

        public LeadExpenseController(LeadExpenseService leadExpenseService, ILogger<LeadExpenseController> logger)
        {
            _leadExpenseService = leadExpenseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _leadExpenseService.GetByIdLeadExpenseAsync(id);

            var model = new LeadExpenseFormModel
            {
                Id = expense.Id,
                Amount = expense.Amount,
                LeadId = expense.TriggerLeadHisto.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LeadExpenseFormModel formModel)
        {
            try
            {
                var updatedExpense = new LeadExpenseForm
                {
                    Amount = formModel.Amount,
                    CreatedAt = DateTime.UtcNow,
                    TriggerLeadHisto = formModel.LeadId
                };
                var result = await _leadExpenseService.UpdateAsync(updatedExpense);
                if (result == null)
                {
                    TempData["Error"] = "Échec de la mise à jour de la depense.";
                }
                else
                {
                    TempData["Message"] = "Dépense mise à jour avec succès.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la dépense.");
                TempData["Error"] = $"Erreur lors de la mise à jour de la dépense : {ex.Message}";
            }

            return RedirectToAction("Details", "Lead", new { id = formModel.LeadId });
        }
    }
}
