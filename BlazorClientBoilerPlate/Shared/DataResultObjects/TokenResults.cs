using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorClientBoilerPlate.Shared.DataResultObjects
{
    public class TokenResults
    {
        public string Token { get; set; }
        public string Refreshtoken { get; set; }
        [JsonPropertyName("RefreshTokenExpiryTime")]
        public DateTime Expiration { get; set; }
    }
}
