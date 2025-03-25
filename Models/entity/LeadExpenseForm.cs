using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace crmcsharp.Models.entity;
public class LeadExpenseForm
{
    public LeadExpenseForm()
    {
    }
    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public int TriggerLeadHisto { get; set; }
}