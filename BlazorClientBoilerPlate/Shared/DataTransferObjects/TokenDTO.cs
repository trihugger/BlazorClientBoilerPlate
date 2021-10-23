using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorClientBoilerPlate.Shared.DataTransferObjects
{
    public class TokenDTO
    {
        [JsonPropertyName("email")]
        public string Username { get; private set; }
        [JsonPropertyName("password")]
        public string Password { get; private set; }

        public TokenDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
