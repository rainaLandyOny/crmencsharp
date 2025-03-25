using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using crmcsharp.Models.entity;

namespace crmcsharp.Services
{
    public class TicketExpenseService
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<TicketExpenseService> _logger;


        public TicketExpenseService(HttpClient httpClient, ILogger<TicketExpenseService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:8080/api/ticket-expenses/");
            _logger = logger;

        }

        // Récupérer une dépense par son ID
        public async Task<TicketExpense> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Envoi d'une requête GET à l'URL : {_httpClient.BaseAddress}{id}");

                string response = await _httpClient.GetStringAsync($"{id}");

                _logger.LogInformation("Réponse reçue :");
                _logger.LogInformation(response);

                TicketExpense ticketExpense = JsonConvert.DeserializeObject<TicketExpense>(response);

                _logger.LogInformation("Détails de la dépense :");
                _logger.LogInformation($"ID: {ticketExpense.Id}");
                _logger.LogInformation($"Montant: {ticketExpense.Amount}");
                _logger.LogInformation($"Date de création: {ticketExpense.CreatedAt}");

                return ticketExpense;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de la récupération de la dépense : {ex.Message}");
                throw new ApplicationException("Erreur lors de la récupération de la dépense.", ex);
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
       
       
        public async Task<TicketExpense> UpdateAsync(int id, TicketExpense updatedTicketExpense)
        {
            try
            {
                // Créer le contenu JSON à partir de l'objet updatedTicketExpense
                var jsonContent = JsonContent.Create(updatedTicketExpense);

                // Appeler l'API Java pour mettre à jour la dépense
                var response = await _httpClient.PutAsync($"http://localhost:8080/api/ticket-expenses/modif/{id}", jsonContent);

                // Vérifier si la requête a réussi
                if (response.IsSuccessStatusCode)
                {
                    // Désérialiser la réponse en TicketExpense
                    var updatedExpense = await response.Content.ReadFromJsonAsync<TicketExpense>();
                    return updatedExpense;
                }
                else
                {
                    // Lire le message d'erreur de la réponse
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erreur HTTP : {response.StatusCode}, Détails : {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Journaliser l'erreur et relancer une exception personnalisée
                _logger.LogError(ex, "Erreur lors de la communication avec l'API Java.");
                throw new Exception("Erreur lors de la communication avec l'API Java.", ex);
            }
            catch (Exception ex)
            {
                // Capturer toute autre exception inattendue
                _logger.LogError(ex, "Une erreur inattendue s'est produite.");
                throw new Exception("Une erreur inattendue s'est produite.", ex);
            }
        }
    }
}