using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClientBoilerPlate.Shared.DataTransferObjects
{
    public class RefreshTokenDTO
    {
        public string RefreshToken { get; set; }

        public RefreshTokenDTO(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
