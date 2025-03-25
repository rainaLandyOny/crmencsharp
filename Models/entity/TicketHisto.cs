using System;
using System.ComponentModel.DataAnnotations;

namespace crmcsharp.Models.entity
{
    public class TicketHisto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public User Manager { get; set; }
        public User Employee { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }   
}