using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using crmcsharp.Models.entity;

namespace crmcsharp.Services
{
    public class LeadExpenseService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LeadExpenseService> _logger;

        public LeadExpenseService(HttpClient httpClient, ILogger<LeadExpenseService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:8080/api/lead-expenses/");
            _logger = logger;
        }

        // Récupérer une dépense par son ID
        public async Task<LeadExpense> GetByIdAsync(int id)
        {
            try
            {
                string response = await _httpClient.GetStringAsync($"{id}");
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                return JsonConvert.DeserializeObject<LeadExpense>(response,settings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération de la dépense.", ex);
            }
        }

        public async Task<LeadExpense> GetByIdLeadExpenseAsync(int id)
        {
            try
            {
                string response = await _httpClient.GetStringAsync($"leadExpense/{id}");
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                return JsonConvert.DeserializeObject<LeadExpense>(response,settings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération de la dépense.", ex);
            }
        }
        // Récupérer toutes les dépenses
        public async Task<List<LeadExpense>> GetAllAsync()
        {
            try
            {
                string response = await _httpClient.GetStringAsync("all");
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
                return JsonConvert.DeserializeObject<List<LeadExpense>>(response,settings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de la récupération des dépenses : {ex.Message}");
                throw new ApplicationException("Erreur lors de la récupération des dépenses.", ex);
            }
        }

        // Ajouter une nouvelle dépense
        public async Task<LeadExpense> CreateAsync(LeadExpense newExpense)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("create", newExpense);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LeadExpense>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erreur HTTP : {response.StatusCode}, Détails : {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la dépense.");
                throw new Exception("Erreur lors de la création de la dépense.", ex);
            }
        }

        public async Task<LeadExpense> UpdateAsync(LeadExpenseForm updatedExpense)
        {
            try
            {
                // 1. Création du DTO sans l'ID
                var updateData = new
                {
                    Amount = updatedExpense.Amount,
                    CreatedAt = updatedExpense.CreatedAt,
                    LeadId = updatedExpense.TriggerLeadHisto

                };

                // 2. Sérialisation
                var jsonContent = JsonContent.Create(updateData);

                // 3. Envoi de la requête
                var response = await _httpClient.PutAsync("update", jsonContent); // URL simplifiée

                // 4. Vérification
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<LeadExpense>(responseContent, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour");
                throw;
            }
        }

        public async Task<decimal> GetDateAsync(DateTime date1, DateTime date2)
        {
            try
            {
                // Convertir en UTC pour éviter les problèmes de timezone
                DateTime utcDate1 = date1.ToUniversalTime();
                DateTime utcDate2 = date2.ToUniversalTime();

                // Formatage des dates en ISO 8601 sans fractions de seconde
                string date1Str = utcDate1.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string date2Str = utcDate2.ToString("yyyy-MM-ddTHH:mm:ssZ");

                string url = $"total?date1={Uri.EscapeDataString(date1Str)}&date2={Uri.EscapeDataString(date2Str)}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                // Vérifier que la réponse est réussie
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    // Format cohérent avec le format d'envoi
                    DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" 
                };

                return JsonConvert.DeserializeObject<decimal>(responseContent, settings);
            }
            catch (HttpRequestException httpEx)
            {
                throw new ApplicationException($"Erreur HTTP lors de l'appel à l'API: {httpEx.StatusCode}", httpEx);
            }
            catch (JsonException jsonEx)
            {
                throw new ApplicationException("Erreur lors de la désérialisation de la réponse", jsonEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des tickets historiques.", ex);
            }
        }
       


        // Mettre à jour une dépense
        
        // Supprimer une dépense
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ApplicationException("Dépense non trouvée.");
                }
                else
                {
                    throw new ApplicationException($"Erreur lors de la suppression de la dépense. Code de statut : {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la dépense.");
                throw new ApplicationException("Erreur lors de la suppression de la dépense.", ex);
            }
        }
    }
}
