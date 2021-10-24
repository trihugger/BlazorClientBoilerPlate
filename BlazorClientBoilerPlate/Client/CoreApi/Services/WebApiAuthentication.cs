using BlazorClientBoilerPlate.Client.API.BaseApi;
using BlazorClientBoilerPlate.Client.API.Constants;
using BlazorClientBoilerPlate.Shared.DataModels;
using BlazorClientBoilerPlate.Shared.DataResultObjects;
using BlazorClientBoilerPlate.Shared.DataTransferObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClientBoilerPlate.Client.API.Services
{
    public class WebApiAuthentication
    {
        private readonly WebApiClientFactory _webApiClientFactory;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private WebApiClient _webApiClient;

        public User User { get; set; } = null;

        public bool IsAuthenticated 
        { 
            get
            {
                return User != null;
            }
        }

        public WebApiAuthentication(WebApiClientFactory webApiClientFactory, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider)
        {
            _webApiClientFactory = webApiClientFactory;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task Initialize()
        {
           // TODO: stuff to initialize see if we need local storage
        }

        public async Task<bool> AuthenticateAsync(Login login, CancellationToken cancellationToken = default)
        {
            _webApiClient = _webApiClient ?? _webApiClientFactory.Create();
            TokenDTO tokenDTO = new TokenDTO(login.Username, login.Password);
            var response = await _webApiClient.ExecuteAsync<GetAllResult<TokenResults>, TokenDTO>(HttpMethod.Post, WebApiEndpoints.TokenEndpoint, cancellationToken, tokenDTO).ConfigureAwait(false);
            if(response != null)
            {
                var responseCurrentUser = await _webApiClient.ExecuteAsync<string>(HttpMethod.Get, WebApiEndpoints.UsersCurrentEndpoint, cancellationToken, response.Data.Token).ConfigureAwait(false);
                if(!string.IsNullOrEmpty(responseCurrentUser))
                {
                    var responseUser = await _webApiClient.ExecuteAsync<GetAllResults<UserResult>>(HttpMethod.Get, WebApiEndpoints.UsersEndpoint.AddParameters(new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("", responseCurrentUser)}), cancellationToken, response.Data.Token).ConfigureAwait(false);
                    if(responseUser.Data[0].Id != Guid.Empty)
                    {
                        User = new User(responseUser.Data[0].Id, responseUser.Data[0].FirstName, responseUser.Data[0].LastName, responseUser.Data[0].Email, response.Data.Token, response.Data.Refreshtoken, response.Data.Expiration);
                        (_authenticationStateProvider as AppAuthStateProvider).LoginNotify();
                        return true;
                    }
                    User = new User(response.Data.Token, response.Data.Refreshtoken, response.Data.Expiration);
                    return false;
                }
                User = new User(response.Data.Token, response.Data.Refreshtoken, response.Data.Expiration);
                return false;
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
            _webApiClient = _webApiClient ?? _webApiClientFactory.Create();
            RefreshTokenDTO refreshTokenDTO = new RefreshTokenDTO(User.RefreshToken); // TODO: Update RefreshTokenDTO and TokenEndpoint is correct
            var response = await _webApiClient.ExecuteAsync<GetAllResult<TokenResults>, RefreshTokenDTO>(HttpMethod.Post, WebApiEndpoints.TokenEndpoint, cancellationToken, refreshTokenDTO).ConfigureAwait(false);
            if(response != null)
            {
                var token = response.Data;
                User.Token = token.Token;
                User.RefreshToken = token.Refreshtoken;
                User.TokenExpireDate = token.Expiration;
                return true;
            }
            return false;
        }
    }
}
