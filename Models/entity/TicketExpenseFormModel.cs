namespace crmcsharp.Models.entity
{
    public class TicketExpenseFormModel
    {
        public int Id { get; set; } // ID de la dépense
        public decimal Amount { get; set; } // Montant de la dépense
        public int TicketId { get; set; } // ID du ticket associé
    }
}