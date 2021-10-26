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
        private JwtSettings _jwtconfig = new JwtSettings();
        private readonly IConfiguration _configuration;

        public User? User { get; set; } = null;

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
            var jwtsettings = _configuration.GetSection("JwtSettings:key");
            _jwtconfig.Key = jwtsettings.Value;
        }

        public void Initialize()
        {
            // TODO: stuff to initialize see if we need local storage
        }

        public async Task<bool> AuthenticateAsync(Login login, CancellationToken cancellationToken = default)
        {
            _webApiClient = _webApiClientFactory.Create();
            TokenDTO tokenDTO = new TokenDTO(login.Username, login.Password);
            var response = await _webApiClient.ExecuteAsync<GetAllResult<TokenResults>, TokenDTO>(HttpMethod.Post, WebApiEndpoints.TokenEndpoint, cancellationToken, tokenDTO).ConfigureAwait(false);
            if (response != null)
            {
                var principal = GetPrincipalClaims(response.Data);
                SetUserFromClaims(principal, response.Data);
                ((AppAuthStateProvider)_authenticationStateProvider).LoginNotify(principal);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            User = default;
            ((AppAuthStateProvider)_authenticationStateProvider).LogoutNotify();
            _navigationManager.NavigateTo("/login");
        }

        public async Task<bool> RefreshToken(CancellationToken cancellationToken)
        {
            _webApiClient = _webApiClientFactory.Create();
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

        #region Process Token and Claims
        private ClaimsPrincipal GetPrincipalClaims(TokenResults tokenResults)
        {
            var claims = new List<Claim>();
            var payload = tokenResults.Token.Split(".")[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            claims.AddRange(keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            claims.Add(new Claim("Token", tokenResults.Token));
            claims.Add(new Claim("RefreshToken", tokenResults.Refreshtoken));
            claims.Add(new Claim("TokenExpiration", tokenResults.Expiration.ToString("MM/dd/yyyy hh:mm:ss")));
            // claims = GetRoles(claims);             // TODO: Get Roles
            // TODO: Get RoleClaims

            // TODO: For RoleClaims - setup constants similar to server
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return principal;
        }

        private async Task<List<Claim>> GetRoles(List<Claim> claims, CancellationToken cancellationToken = default)
        {
            var response = await _webApiClient.ExecuteAsync<GetAllResult<UserRolesResults>>(HttpMethod.Get, WebApiEndpoints.UsersRolesEndpoint(User!.Id), cancellationToken, User.Token);
            if(response != null)
            {
                foreach (var role in response.Data.UserRoles)
                {
                    if(role.Enabled) claims.Add(new Claim(ClaimTypes.Role, role.RoleName.ToString()));
                }
            }
            return claims;
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

        private void SetUserFromClaims(ClaimsPrincipal principal, TokenResults token)
        {
            var userid = principal.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier).Value ?? string.Empty;
            var firstname = principal.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Name).Value ?? string.Empty;
            var lastname = principal.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Surname).Value ?? string.Empty;
            var email = principal.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Email).Value ?? string.Empty;
            User = new User(
                userid,
                firstname,
                lastname,
                email,
                token.Token,
                token.Refreshtoken,
                token.Expiration
                );
        }


        #endregion
    }
}