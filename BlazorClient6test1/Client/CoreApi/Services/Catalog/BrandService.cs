using BlazorClient6test1.Client.API.BaseApi;
using BlazorClient6test1.Client.API.Constants;
using BlazorClient6test1.Shared.DataParameterObject;
using BlazorClient6test1.Shared.DataResultObjects;
using BlazorClient6test1.Shared.DataTransferObjects;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BlazorClient6test1.Client.API.Services.Catalog
{
    public interface IBrandService
    {
        Task<BrandDTO> GetById(Guid id);
        Task<GetAllResults<BrandDTO>> GetAll(BrandFilter filter, CancellationToken cancellationToken = default);
    }

    public class BrandService : IBrandService
    {
        private readonly WebApiClient _webApiService;
        private readonly WebApiAuthentication _webApiAuthentication;
       
        public BrandService(WebApiClient webApiService, WebApiAuthentication webApiAuthentication)
        {
            _webApiService = webApiService;
            _webApiAuthentication = webApiAuthentication;
        }

        public async Task<GetAllResults<BrandDTO>> GetAll(BrandFilter filter, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize<BrandFilter>(filter);
            KeyValuePair<string, string>[] parameters = new KeyValuePair<string, string>[]{new KeyValuePair<string, string>("filter", json)};
            var response = await _webApiService.ExecuteAsync<GetAllResults<BrandDTO>>(HttpMethod.Get, WebApiEndpoints.BrandsV1Endpoint.AddParameters(parameters), cancellationToken, _webApiAuthentication.User.Token);
            return response;
        }

        public async Task<BrandDTO> GetById(Guid id)
        {
            // TODO implement API call to get by ID
            return new BrandDTO();
        }
    }
}
