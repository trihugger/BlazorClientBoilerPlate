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
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Test"),
                new Claim(ClaimTypes.Email, "test@test.com")
            }, "Fake Authentication");

            claimsPrincipal = new ClaimsPrincipal(identity);
            
            if(principal != null)
                claimsPrincipal = principal;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void LogoutNotify()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            claimsPrincipal = new ClaimsPrincipal(anonymous);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
