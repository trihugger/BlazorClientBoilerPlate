using BlazorClientBoilerPlate.Client.API.BaseApi;
using BlazorClientBoilerPlate.Client.API.Constants;
using BlazorClientBoilerPlate.Shared.DataModels;
using BlazorClientBoilerPlate.Shared.DataResultObjects;
using BlazorClientBoilerPlate.Shared.DataTransferObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BlazorClientBoilerPlate.Client.API.Services
{
    public class WebApiAuthentication
    {
        private readonly WebApiClientFactory _webApiClientFactory;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private WebApiClient _webApiClient;
        private readonly JwtSettings _jwtconfig;
        private readonly IConfiguration _configuration;

        public User User { get; set; } = null;

        public bool IsAuthenticated
        {
            get
            {
                return User != null;
            }
        }

        public WebApiAuthentication(WebApiClientFactory webApiClientFactory, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, IConfiguration configuration)
        {
            _webApiClientFactory = webApiClientFactory;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
            _configuration = configuration;
            //_jwtconfig.Key = configuration["JwtSettings:key"].ToString();
        }

        public async Task Initialize()
        {
            // TODO: stuff to initialize see if we need local storage
        }

        public async Task<bool> AuthenticateAsync(Login login, CancellationToken cancellationToken = default)
        {
            try
            {
                _webApiClient = _webApiClientFactory.Create();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            TokenDTO tokenDTO = new TokenDTO(login.Username, login.Password);
            var response = await _webApiClient.ExecuteAsync<GetAllResult<TokenResults>, TokenDTO>(HttpMethod.Post, WebApiEndpoints.TokenEndpoint, cancellationToken, tokenDTO).ConfigureAwait(false);
            if (response != null)
            {
                var jwtsettings = _configuration.GetSection("JwtSettings:key");
                var key = jwtsettings.Value;
                var principal = GetPrincipalFromExpiredToken(response.Data.Token, key);
                (_authenticationStateProvider as AppAuthStateProvider).LoginNotify(principal);

                var userid = principal.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value ?? string.Empty;
                var firstname = principal.Claims.Where(t => t.Type == ClaimTypes.Name).FirstOrDefault().Value ?? string.Empty;
                var lastname = principal.Claims.Where(t => t.Type == ClaimTypes.Surname).FirstOrDefault().Value ?? string.Empty;
                var email = principal.Claims.Where(t => t.Type == ClaimTypes.Email).FirstOrDefault().Value;
                User = new User(
                    userid,
                    firstname,
                    lastname,
                    email,
                    response.Data.Token,
                    response.Data.Refreshtoken,
                    response.Data.Expiration
                    );

                return true;
                // var responseCurrentUser = await _webApiClient.ExecuteAsync<string>(HttpMethod.Get, WebApiEndpoints.UsersCurrentEndpoint, cancellationToken, response.Data.Token).ConfigureAwait(false);
                // if (!string.IsNullOrEmpty(responseCurrentUser))
                //{
                //    var responseUser = await _webApiClient.ExecuteAsync<GetAllResults<UserResult>>(HttpMethod.Get, WebApiEndpoints.UsersEndpoint.AddParameters(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("", responseCurrentUser) }), cancellationToken, response.Data.Token).ConfigureAwait(false);
                //    if (responseUser.Data[0].Id != Guid.Empty)
                //    {
                //        User = new User(responseUser.Data[0].Id, responseUser.Data[0].FirstName, responseUser.Data[0].LastName, responseUser.Data[0].Email, response.Data.Token, response.Data.Refreshtoken, response.Data.Expiration);
                        
                //        return true;
                //    }
                //    User = new User(response.Data.Token, response.Data.Refreshtoken, response.Data.Expiration);
                //    return false;
                //}
                //User = new User(response.Data.Token, response.Data.Refreshtoken, response.Data.Expiration);
                //return false;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            User = null;
            _navigationManager.NavigateTo("/login");
            (_authenticationStateProvider as AppAuthStateProvider).LogoutNotify();
        }

        public async Task<bool> RefreshToken(CancellationToken cancellationToken)
        {
            try
            {
                _webApiClient = _webApiClientFactory.Create();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            RefreshTokenDTO refreshTokenDTO = new RefreshTokenDTO(User.RefreshToken); // TODO: Update RefreshTokenDTO and TokenEndpoint is correct
            var response = await _webApiClient.ExecuteAsync<GetAllResult<TokenResults>, RefreshTokenDTO>(HttpMethod.Post, WebApiEndpoints.TokenEndpoint, cancellationToken, refreshTokenDTO).ConfigureAwait(false);
            if (response != null)
            {
                var token = response.Data;
                User.Token = token.Token;
                User.RefreshToken = token.Refreshtoken;
                User.TokenExpireDate = token.Expiration;
                return true;
            }
            return false;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string key)
        {
            // new
            var claims = new List<Claim>();
            var payload = token.Split(".")[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            ClaimsPrincipal principal2 = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuthType"));
            return principal2;

            // old
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidation = tokenValidationParameters(key);
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidation, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                        !jwtSecurityToken.Header.Alg.Equals(
                            SecurityAlgorithms.HmacSha256,
                            StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Token didn't verify");
                }
                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test");
            }
            return default;
        }

        private TokenValidationParameters tokenValidationParameters(string key)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };

            return tokenValidationParameters;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch(base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
