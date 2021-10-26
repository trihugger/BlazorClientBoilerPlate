using BlazorClientBoilerPlate.Client;
using BlazorClientBoilerPlate.Client.API;
using BlazorClientBoilerPlate.Client.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorClientBoilerPlate.CoreApi;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#region Setup Logging Configuration
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Add Logging
builder.Logging.AddProvider(new CustomLoggerProvider());
#endregion

#region Setup Core Application functionality
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore(); //Setup default authorization
builder.Services.AddWebApi(); // WebApiClient Implementation Service Collection
builder.Services.AddScoped<IAlertService, AlertService>(); // Add supporting services for client UI functionality
#endregion

//TODO: consider 3rd party authentication
//builder.Services.AddMsalAuthentication(options =>
//{
//    // Replace with Okta and other services
//    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
//    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://api.id.uri/access_as_user");
//});

#region Setup App
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
#endregion

await builder.Build().RunAsync();
