using BlazorClient6test1.Client.API.BaseApi;
using BlazorClient6test1.Client.API.Services;
using BlazorClient6test1.Client.API.Services.Catalog;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorClient6test1.Client.API
{
    public static class WebApiConfigurator
    {
        public static void AddWebApi(this IServiceCollection services)
        {
            // Add services for each project
            services.AddTransient<IBrandService, BrandService>();

            // https://www.learmoreseekmore.com/2020/10/blazor-webassembly-custom-authentication-from-scratch.html
            services.AddSingleton<AuthenticationStateProvider, AppAuthStateProvider>();
            services.AddSingleton<WebApiAuthentication>();

            // https://codewithmukesh.com/blog/authentication-in-blazor-webassembly/
            // https://josef.codes/you-are-probably-still-using-httpclient-wrong-and-it-is-destabilizing-your-software/
            // Named service
            services.AddHttpClient<WebApiClient>("WebApiClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001/api");
                client.DefaultRequestHeaders.Add("tenantKey", "root"); // 1) make it static here, 2) configurable from the appsettings/api settings, or 3) selected at login.
                client.DefaultRequestVersion = HttpVersion.Version30; //Enable Http3
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact; //Enforce Http3
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            services.AddSingleton<WebApiClientFactory>();
        }
    }
}
