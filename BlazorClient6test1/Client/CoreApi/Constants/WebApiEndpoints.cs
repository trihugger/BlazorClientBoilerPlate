using System.Collections.Generic;

namespace BlazorClient6test1.Client.API.Constants
{
    public class WebApiEndpoints
    {
        public const string BaseAppEndpoint = "https://localhost:5001/api";
        public const string HealthCheckEndpoint = BaseAppEndpoint + "/health";
        public const string TokenEndpoint = BaseAppEndpoint + "/tokens/";
        public const string IdentityEndpoint = BaseAppEndpoint + "/identity";
        public const string IdentityRegisterEndpoint = IdentityEndpoint + "/register";
        public const string RolesEndpoint = BaseAppEndpoint + "/roles";
        public const string RolesAllEndpoint = RolesEndpoint + "/all";
        public string RolesPermissionsEndpoint(Guid roleId) => RolesEndpoint + "/" + roleId + "/permissions";
        public const string UsersEndpoint = BaseAppEndpoint + "/users";
        public const string UsersCurrentEndpoint = UsersEndpoint + "/id";
        public const string TenantEndpoint = BaseAppEndpoint + "/tenants";
        public string TenantByKeyEndpoint(string tenantKey) => TenantEndpoint + "/" + tenantKey;
        public const string BrandsV1Endpoint = BaseAppEndpoint + "/v1/brands";

    }

    public static class WebApiEndpointsExtensions
    {
        public static Uri ToUri(this string endpoint) => new Uri(endpoint);

        public static string AddParameters(this string endpoint, KeyValuePair<string,string>[] parameters)
        {
            endpoint += "?";
            foreach(KeyValuePair<string,string> param in parameters){
                endpoint += param.Key + "=" + param.Value;
            }

            return endpoint;
        }
    }
}
