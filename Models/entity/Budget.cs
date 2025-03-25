using System;

namespace crmcsharp.Models.entity
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int CustomerId { get; set; } 
    }
}
