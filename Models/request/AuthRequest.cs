// Models/LoginModel.cs
namespace crmcsharp.Models.request
{
    using System.Text.Json.Serialization;

    public class AuthRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

}