using Microsoft.AspNetCore.Mvc;
using crmcsharp.Services;
using crmcsharp.Models.entity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace crmcsharp.Controllers
{
    public class TicketExpenseController : Controller
    {
        private readonly TicketExpenseService _ticketExpenseService;
        private readonly ILogger<TicketExpenseController> _logger;

        public TicketExpenseController(TicketExpenseService ticketExpenseService, ILogger<TicketExpenseController> logger)
        {
            _ticketExpenseService = ticketExpenseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int ticketId)
        {
            var expense = await _ticketExpenseService.GetByIdAsync(ticketId);
            if (expense == null)
            {
                TempData["Error"] = "Dépense introuvable.";
                return RedirectToAction("Details", "Ticket", new { id = ticketId });
            }

            var model = new TicketExpenseFormModel
            {
                Id = expense.Id,
                Amount = expense.Amount,
                TicketId = expense.TicketHisto?.Id ?? ticketId
            };

            return View(model);
        }


        // Traite la soumission du formulaire de mise à jour de la dépense
        [HttpPost]
        public async Task<IActionResult> Update(TicketExpenseFormModel formModel)
        {
            try
            {
                // Crée un objet TicketExpense à partir des données du formulaire
                var updatedExpense = new TicketExpense
                {
                    Id = formModel.Id,
                    Amount = formModel.Amount,
                    CreatedAt = DateTime.UtcNow, // Correction ici
                    TicketHisto = new TicketHisto { Id = formModel.TicketId }
                };


                // Appelle le service pour mettre à jour la dépense
                var result = await _ticketExpenseService.UpdateAsync(formModel.TicketId, updatedExpense);

                // Vérifie si la mise à jour a réussi
                if (result == null)
                {
                    TempData["Error"] = "Échec de la mise à jour de la dépense.";
                }
                else
                {
                    TempData["Message"] = "Dépense mise à jour avec succès.";
                }
            }
            catch (Exception ex)
            {
                // Loggue l'erreur et affiche un message d'erreur à l'utilisateur
                _logger.LogError(ex, "Erreur lors de la mise à jour de la dépense.");
                TempData["Error"] = $"Erreur lors de la mise à jour de la dépense : {ex.Message}";
            }

            // Redirige vers la page de détails du ticket
            return RedirectToAction("Details", "Ticket", new { id = formModel.TicketId });
        }
    
    }
}