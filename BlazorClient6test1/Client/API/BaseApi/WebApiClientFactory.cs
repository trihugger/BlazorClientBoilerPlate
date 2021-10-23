namespace BlazorClient6test1.Client.API.BaseApi
{
    public class WebApiClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WebApiClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public WebApiClient Create()
        {
            return _serviceProvider.GetRequiredService<WebApiClient>();
        }
    }
}
