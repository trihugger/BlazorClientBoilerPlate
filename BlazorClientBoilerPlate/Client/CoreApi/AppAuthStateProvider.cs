using BlazorClientBoilerPlate.Client.API.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorClientBoilerPlate.Client.API
{
    public class AppAuthStateProvider : AuthenticationStateProvider
    {
        //Check on this article for certificate authentication
        //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-6.0#implement-an-httpclient-using-a-certificate-and-a-named-httpclient-from-ihttpclientfactory

        private ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public void LoginNotify(ClaimsPrincipal principal)
        {
            claimsPrincipal = principal != null ? principal : GetAnonymous();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void LogoutNotify()
        {
            claimsPrincipal = GetAnonymous();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // TODO: create a has claim method

        private ClaimsPrincipal GetAnonymous()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Test")
            };
            var test = new ClaimsPrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims)));

            return new ClaimsPrincipal(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
