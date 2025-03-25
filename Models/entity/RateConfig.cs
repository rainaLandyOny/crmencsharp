using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace crmcsharp.Models.entity
{
    public class RateConfig
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;

    [JsonPropertyName("rate")]
    [Required(ErrorMessage = "Le taux est obligatoire")]
    [Range(0.01, 100, ErrorMessage = "Le taux doit être entre 0.01 et 100")]
    public decimal Rate { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Today;
}

}