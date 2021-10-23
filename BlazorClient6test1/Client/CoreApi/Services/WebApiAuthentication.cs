using BlazorClient6test1.Client.API.BaseApi;
using BlazorClient6test1.Client.API.Constants;
using BlazorClient6test1.Shared.DataModels;
using BlazorClient6test1.Shared.DataResultObjects;
using BlazorClient6test1.Shared.DataTransferObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClient6test1.Client.API.Services
{
    public class WebApiAuthentication
    {
        private readonly WebApiClientFactory _webApiClientFactory;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

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
            var webApiClient = _webApiClientFactory.Create();
            TokenDTO tokenDTO = new TokenDTO(login.Username, login.Password);
            var response = await webApiClient.ExecuteAsync<GetAllResult<TokenResults>, TokenDTO>(HttpMethod.Post, WebApiEndpoints.TokenEndpoint, cancellationToken, tokenDTO).ConfigureAwait(false);
            if(response != null)
            {
                var responseCurrentUser = await webApiClient.ExecuteAsync<string>(HttpMethod.Get, WebApiEndpoints.UsersCurrentEndpoint, cancellationToken, response.Data.Token).ConfigureAwait(false);
                if(!string.IsNullOrEmpty(responseCurrentUser))
                {
                    var responseUser = await webApiClient.ExecuteAsync<GetAllResults<UserResult>>(HttpMethod.Get, WebApiEndpoints.UsersEndpoint.AddParameters(new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("", responseCurrentUser)}), cancellationToken, response.Data.Token).ConfigureAwait(false);
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
    }
}
