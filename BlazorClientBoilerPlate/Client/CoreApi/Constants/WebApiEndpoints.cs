using System.Collections.Generic;

namespace BlazorClientBoilerPlate.Client.API.Constants
{
    public static class WebApiEndpoints
    {
        public const string BaseAppEndpoint = "https://localhost:5001/api";
        public const string HealthCheckEndpoint = BaseAppEndpoint + "/health";
        public const string TokenEndpoint = BaseAppEndpoint + "/tokens/";
        public const string IdentityEndpoint = BaseAppEndpoint + "/identity";
        public const string IdentityRegisterEndpoint = IdentityEndpoint + "/register";
        public const string RolesEndpoint = BaseAppEndpoint + "/roles";
        public const string RolesAllEndpoint = RolesEndpoint + "/all";
        public static string RolesPermissionsEndpoint(string roleId) => RolesEndpoint + "/" + roleId + "/permissions";
        public const string UsersEndpoint = BaseAppEndpoint + "/users";
        public static string UsersGetByIdEndpoint(string userId) => UsersEndpoint + "/" + userId;
        public static string UsersRolesEndpoint(string userId) => UsersEndpoint + "/" + userId + "/roles";
        public static string UsersPermissionsEndpoint(string userId) => UsersEndpoint + "/" + userId + "/permissions";
        public const string UsersCurrentEndpoint = UsersEndpoint + "/id";
        public const string TenantEndpoint = BaseAppEndpoint + "/tenants";
        public static string TenantByKeyEndpoint(string tenantKey) => TenantEndpoint + "/" + tenantKey;
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
