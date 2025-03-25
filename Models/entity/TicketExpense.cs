using System;
using System.ComponentModel.DataAnnotations;

namespace crmcsharp.Models.entity
{
    public class TicketExpense
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public TicketHisto TicketHisto { get; set; }
    }   
}