using BlazorClientBoilerPlate.Client.API.Constants;
using BlazorClientBoilerPlate.Client.API.Services;
using BlazorClientBoilerPlate.Shared.DataModels;
using BlazorClientBoilerPlate.Shared.DataResultObjects;
using BlazorClientBoilerPlate.Shared.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Polly;
using Polly.Extensions.Http;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorClientBoilerPlate.Client.API.BaseApi
{
    public class WebApiClient : IWebApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly WebApiAuthentication _webApiAuthentication;
        private readonly ILogger _logger;
        private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        public WebApiClient(HttpClient httpClient, WebApiAuthentication webApiAuthentication, ILogger logger)
        {
            _logger = logger;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _webApiAuthentication = webApiAuthentication;
        }

        public async Task<T> ExecuteAsync<T>(HttpMethod method, string endpoints, CancellationToken cancellationToken, string token = "")
        {
            return await ExecuteAsync<T,T>(method, endpoints, cancellationToken, token: token);
        }

        public async Task<T1> ExecuteAsync<T1, T2>(HttpMethod method, string endpoints, CancellationToken cancellationToken, T2 payload = default, string token = "")
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            HttpRequestMessage request;

            if(payload is null)
                request = CreateRequest(method, endpoints);
            else
                request = CreateRequest<T2>(method, endpoints, payload);

            if (!string.IsNullOrEmpty(token))
                request.Headers.Add("Authorization", $"Bearer {token}");

            var _httpRequestPolicy = GetRetryPolicy(_logger, _webApiAuthentication);

            using (var result = await _httpRequestPolicy.ExecuteAsync(async () => await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false)))
            {
                using (var contentStream = await result.Content.ReadAsStreamAsync())
                {
                    return await JsonSerializer.DeserializeAsync<T1>(contentStream, _serializerOptions, cancellationToken);
                }
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            _logger.LogInformation("Time spend on this query was " + ts.TotalMilliseconds);
        }

        private static HttpRequestMessage CreateRequest<T>(HttpMethod method, string endpoint, T payload = default)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, endpoint);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("tenantKey", "root");
            string json = JsonSerializer.Serialize<T>(payload);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;
            return request;
        } 

        private static HttpRequestMessage CreateRequest(HttpMethod method, string endpoint)
        {
            return new HttpRequestMessage(method, endpoint);
        }

        // Retry and Gate Policies for persistency
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger, WebApiAuthentication webApiAuthentication)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: async (response, retryDelay, retryCount, context) =>
                    {
                        context["message"] = context["message"] + $"Received: {response.Result.StatusCode}, retryCount: {retryCount}, delaying: {retryDelay.Seconds} seconds"; // Allows to use this on the context it was sent for the UI to send alerts
                        logger.LogWarning("WebApi Client is retrying for the " + retryCount + ", due to " + response.Exception.Message);
                        if(response.Result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            bool refreshed = await webApiAuthentication.RefreshToken(CancellationToken.None).ConfigureAwait(false);
                            if (!refreshed)
                                throw response.Exception;
                        };
                    });
        }
    }

    public interface IWebApiClient
    {
        Task<T> ExecuteAsync<T>(HttpMethod method, string endpoints, CancellationToken cancellationToken, string token = "");
        Task<T1> ExecuteAsync<T1, T2>(HttpMethod method, string endpoints, CancellationToken cancellationToken, T2 payload = default, string token = "");
    }


}
