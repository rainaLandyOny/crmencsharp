using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using crmcsharp.Models.entity;
using System.Text.Json;

namespace crmcsharp.Services
{
    public class RateConfigService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RateConfigService> _logger;
        private const string BaseUrl = "http://localhost:8080/api/rate-configs";

        public RateConfigService(HttpClient httpClient, ILogger<RateConfigService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Configuration du client HTTP
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<RateConfig>> GetAllRateConfigsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryParams = new System.Collections.Specialized.NameValueCollection();
                
                if (startDate.HasValue)
                    queryParams.Add("startDate", startDate.Value.ToString("yyyy-MM-dd"));
                
                if (endDate.HasValue)
                    queryParams.Add("endDate", endDate.Value.ToString("yyyy-MM-dd"));
                
                var queryString = ToQueryString(queryParams);
                var requestUrl = $"{BaseUrl}{queryString}";
                
                _logger.LogInformation($"Fetching rate configs from: {requestUrl}");
                
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<List<RateConfig>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rate configs");
                throw;
            }
        }

        public async Task<RateConfig?> CreateRateConfigAsync(RateConfig rateConfig)
        {
            try
            {
                using var jsonContent = JsonContent.Create(rateConfig);
                
                var response = await _httpClient.PostAsync(BaseUrl, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    // Log du statut d'erreur (à adapter selon votre système de logging)
                    Console.WriteLine($"Erreur HTTP: {response.StatusCode}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Pour gérer les différences de casse
                };
                
                return System.Text.Json.JsonSerializer.Deserialize<RateConfig>(responseContent, options);
            }
            catch (HttpRequestException httpEx)
            {
                // Log spécifique pour les erreurs HTTP
                Console.WriteLine($"Erreur de requête HTTP: {httpEx.Message}");
                return null;
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                // Log spécifique pour les erreurs de désérialisation
                Console.WriteLine($"Erreur de désérialisation JSON: {jsonEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log général
                Console.WriteLine($"Erreur inattendue: {ex.Message}");
                return null;
            }
        }

        private string ToQueryString(System.Collections.Specialized.NameValueCollection nvc)
        {
            if (nvc == null || nvc.Count == 0) return string.Empty;
            
            var array = new List<string>();
            foreach (string key in nvc.Keys)
            {
                array.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(nvc[key])}");
            }
            return "?" + string.Join("&", array);
        }
    }
}