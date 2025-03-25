using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using crmcsharp.Models.entity;

namespace crmcsharp.Services
{
    public class LeadService
    {
        private readonly HttpClient _httpClient;

        public LeadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:8080/api/leads/");
        }

        public async Task<List<TriggerLeadHisto>> GetLeadsAsync()
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

                return JsonConvert.DeserializeObject<List<TriggerLeadHisto>>(response, settings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des leads.", ex);
            }
        }

        public async Task<List<TriggerLeadHisto>> GetDateAsync(DateTime date1, DateTime date2)
        {
            try
            {
                // Convertir en UTC pour éviter les problèmes de timezone
                DateTime utcDate1 = date1.ToUniversalTime();
                DateTime utcDate2 = date2.ToUniversalTime();

                // Formatage des dates en ISO 8601 sans fractions de seconde
                string date1Str = utcDate1.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string date2Str = utcDate2.ToString("yyyy-MM-ddTHH:mm:ssZ");

                string url = $"condition?date1={Uri.EscapeDataString(date1Str)}&date2={Uri.EscapeDataString(date2Str)}";

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

                return JsonConvert.DeserializeObject<List<TriggerLeadHisto>>(responseContent, settings);
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

        public async Task<TriggerLeadHisto> GetLeadByIdAsync(int id)
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

                return JsonConvert.DeserializeObject<TriggerLeadHisto>(response,settings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du lead.", ex);
            }
        }

        public async Task<bool> DeleteLead(int leadId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"delete-lead/{leadId}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ApplicationException("Lead non trouvé.");
                }
                else
                {
                    throw new ApplicationException($"Erreur lors de la suppression du lead. Code : {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la suppression du lead.", ex);
            }
        }
    }
}
