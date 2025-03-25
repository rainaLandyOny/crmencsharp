using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace crmcsharp.Models.entity;
public class Lead
{
    public int LeadId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(meeting-to-schedule|scheduled|archived|success|assign-to-sales)$", ErrorMessage = "Invalid status")]
    public string? Status { get; set; }

    public string? Phone { get; set; }
    public string? MeetingId { get; set; }
    public bool? GoogleDrive { get; set; }
    public string? GoogleDriveFolderId { get; set; }
    public DateTime CreatedAt { get; set; }

    public int? ManagerId { get; set; }
    public int? EmployeeId { get; set; }
    public int? CustomerId { get; set; }
}

