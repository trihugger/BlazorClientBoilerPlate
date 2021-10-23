using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorClient6test1.Shared.DataResultObjects
{
    public class TokenResults
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("refreshToken")]
        public string Refreshtoken { get; set; }
        [JsonPropertyName("refreshTokenExpiryTime")]
        public DateTime Expiration { get; set; }
    }
}
