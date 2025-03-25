using System;
using System.ComponentModel.DataAnnotations;

namespace crmcsharp.Entities
{
    public class TicketExpense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.00, 9999999.99, ErrorMessage = "Amount must be between 0.00 and 9,999,999.99")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int TicketHistoId { get; set; }
    }
}