using System;
using System.Collections.Generic;

namespace crmcsharp.Models.entity
{
    public class OAuthUser
    {
        public int Id { get; set; }
        public List<string> GrantedScopes { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenIssuedAt { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenIssuedAt { get; set; } // Nullable
        public DateTime? RefreshTokenExpiration { get; set; } // Nullable
        public string Email { get; set; }
    }
}