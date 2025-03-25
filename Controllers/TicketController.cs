using Microsoft.AspNetCore.Mvc;
using crmcsharp.Services;
using System.Threading.Tasks;

namespace crmcsharp.Controllers
{
    public class TicketController : Controller
    {
        private readonly TicketService _ticketService;
        private readonly TicketExpenseService _ticketExpenseService;

        public TicketController(TicketService ticketService, TicketExpenseService ticketExpenseService)
        {
            _ticketService = ticketService;
            _ticketExpenseService = ticketExpenseService;
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Récupérer le ticket par son ID
                var ticket = await _ticketService.GetTicketByIdAsync(id);

                if (ticket == null)
                {
                    TempData["Error"] = "Ticket non trouvé.";
                    return RedirectToAction("Index");
                }

                // Récupérer les dépenses associées au ticket
                var ticketExpenses = await _ticketExpenseService.GetByIdAsync(id);

                // Prendre la dépense la plus récente (si plusieurs)
                // Passer les données à la vue
                ViewBag.LatestExpense = ticketExpenses;

                return View(ticket);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de la récupération des détails du ticket : {ex.Message}";
                return RedirectToAction("Index");
            }
        }



        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetTicketsAsync();
            return View(tickets);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ticketId)
        {
            try
            {
                bool isDeleted = await _ticketService.DeleteTicket(ticketId);

                if (isDeleted)
                {
                    TempData["Message"] = "Ticket supprimé avec succès.";
                }
                else
                {
                    TempData["Error"] = "Échec de la suppression du ticket.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur lors de la suppression du ticket : {ex.Message}";
            }

            // Rediriger vers la liste des tickets
            return RedirectToAction("Index");
        }

    }
}