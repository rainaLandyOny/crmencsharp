using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    // Constructeur : initialise le client HTTP avec l'URL de base de l'API
    public ApiClient(string baseUrl)
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl.TrimEnd('/') + "/"; // Assure que l'URL de base se termine par un "/"
    }

    // Méthode générique pour effectuer une requête GET
    public async Task<T> GetAsync<T>(string endpoint, System.Text.Json.JsonSerializerOptions jsonOptions)
    {
        try
        {
            var fullUrl = $"{_baseUrl}{endpoint.TrimStart('/')}"; // Concatène l'URL de base et l'endpoint
            Console.WriteLine($"Envoi de la requête GET à : {fullUrl}"); // Log de l'URL

            var response = await _httpClient.GetAsync(fullUrl);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Réponse brute : {responseBody}"); // Log de la réponse brute

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur API: {response.StatusCode}, {responseBody}");
            }

            return JsonConvert.DeserializeObject<T>(responseBody); // Désérialise la réponse JSON
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la requête GET : {ex.Message}"); // Log de l'erreur
            throw; // Relance l'exception pour la gestion dans le contrôleur
        }
    }

    // Méthode générique pour effectuer une requête POST
    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var fullUrl = $"{_baseUrl}{endpoint.TrimStart('/')}";
            Console.WriteLine($"Envoi de la requête POST à : {fullUrl}");

            var json = JsonConvert.SerializeObject(data); // Convertit l'objet C# en JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(fullUrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Réponse brute : {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur API: {response.StatusCode}, {responseBody}");
            }

            return JsonConvert.DeserializeObject<T>(responseBody); // Désérialise la réponse JSON
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la requête POST : {ex.Message}");
            throw;
        }
    }
}