using System;

namespace crmcsharp.Models.entity
{
    public class TriggerLeadHisto
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public User Employee { get; set; }

        public string Status { get; set; }

        public string MeetingId { get; set; }

        public bool GoogleDrive { get; set; }

        public string GoogleDriveFolderId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DeleteAt { get; set; }

        // Constructeur par défaut
        public TriggerLeadHisto()
        {
        }

        // Constructeur avec tous les champs
        public TriggerLeadHisto(int id, Customer customer, User user, string name, string phone, User employee,
                                string status, string meetingId, bool googleDrive, string googleDriveFolderId,
                                DateTime createdAt, DateTime? deleteAt)
        {
            Id = id;
            Customer = customer;
            User = user;
            Name = name;
            Phone = phone;
            Employee = employee;
            Status = status;
            MeetingId = meetingId;
            GoogleDrive = googleDrive;
            GoogleDriveFolderId = googleDriveFolderId;
            CreatedAt = createdAt;
            DeleteAt = deleteAt;
        }
    }
}