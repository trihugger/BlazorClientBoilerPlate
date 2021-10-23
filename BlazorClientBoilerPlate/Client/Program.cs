using BlazorClientBoilerPlate.Client;
using BlazorClientBoilerPlate.Client.API;
using BlazorClientBoilerPlate.Client.API.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Add Logging

builder.Services.AddWebApi(); // WebApiClient Implementation Service Collection

builder.Services.AddAuthorizationCore(); // Add Authorization Core

builder.Services.AddScoped<IAlertService, AlertService>(); // Add supporting services for client UI functionality

//TODO: consider 3rd party authentication
//builder.Services.AddMsalAuthentication(options =>
//{
//    // Replace with Okta and other services
//    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
//    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://api.id.uri/access_as_user");
//});

await builder.Build().RunAsync();
