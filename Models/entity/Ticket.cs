using System;
using System.ComponentModel.DataAnnotations;

namespace crmcsharp.Models.entity
{
    public class Ticket
    {
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public string? Subject { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(open|assigned|on-hold|in-progress|resolved|closed|reopened|pending-customer-response|escalated|archived)$", ErrorMessage = "Invalid status")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [RegularExpression("^(low|medium|high|closed|urgent|critical)$", ErrorMessage = "Invalid priority")]
        public string? Priority { get; set; }

        public int? ManagerId { get; set; }
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
