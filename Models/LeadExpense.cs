using System;
using System.ComponentModel.DataAnnotations;

namespace crmcsharp.Entities
{
    public class LeadExpense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.00, 9999999.99, ErrorMessage = "Amount must be between 0.00 and 9999999.99")]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "TriggerLeadHistoId is required")]
        public int TriggerLeadHistoId { get; set; }
    }
}
