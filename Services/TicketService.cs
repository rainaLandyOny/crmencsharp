using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using crmcsharp.Models.entity;

namespace crmcsharp.Services
{
    public class TicketService
    {
        private readonly HttpClient _httpClient;

        public TicketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:8080/api/tickets/");
        }

        public async Task<List<TicketHisto>> GetTicketsAsync()
        {
            try
            {
                string response = await _httpClient.GetStringAsync("all");
                List<TicketHisto> ticketHistos = JsonConvert.DeserializeObject<List<TicketHisto>>(response);
                return ticketHistos;
            }
            catch (Exception ex)
            {
                // Gérer les erreurs (journalisation, etc.)
                throw new ApplicationException("Erreur lors de la récupération des tickets.", ex);
            }
        }

       public async Task<bool> DeleteTicket(int ticketId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"delete-ticket/{ticketId}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    string content = await response.Content.ReadAsStringAsync();
                    throw new ApplicationException($"Erreur lors de la suppression du ticket. Code: {(int)response.StatusCode} - {response.ReasonPhrase}. Contenu: {content}");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la suppression du ticket.", ex);
            }
        }

        public async Task<List<TicketHisto>> GetDateAsync(DateTime date1, DateTime date2)
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

                return JsonConvert.DeserializeObject<List<TicketHisto>>(responseContent, settings);
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


        public async Task<TicketHisto> GetTicketByIdAsync(int id)
        {
            try
            {
                string response = await _httpClient.GetStringAsync($"{id}");
                TicketHisto ticket = JsonConvert.DeserializeObject<TicketHisto>(response);
                return ticket;
            }
            catch (Exception ex)
            {
                // Gérer les erreurs (journalisation, etc.)
                throw new ApplicationException("Erreur lors de la récupération des détails du ticket.", ex);
            }
        }
    }

}