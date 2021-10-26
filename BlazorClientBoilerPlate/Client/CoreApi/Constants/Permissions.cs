using System.ComponentModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BlazorClientBoilerPlate.Client.CoreApi.Constants
{
    // Blazor WebAssembly Authorization Docs
    // https://docs.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-6.0
    public static class Permissions
    {
        public static readonly string IsAdmin = "IsAdmin";
        public static readonly string IsManager = "IsManager";
        public static readonly string AccessBrands = "AccessBrands";
        public static readonly string Authenticated = "Authenticated";

        public static AuthorizationPolicy IsAdminPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Administrator", "SuperUser")
                .Build();
        }

        public static AuthorizationPolicy AccessBrandsPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Loan Operations") // Claim of type Role
                .RequireClaim(ClaimTypes.GroupSid, new string[] { Brands.View, Brands.Search }) // Claim of type GroupSid
                .Build();
        }

        public static AuthorizationPolicy AuthenticatedPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        }

        // on Razor:
        // @attribute [Authorize(Policy = Policies.IsAdmin)] // for page attribute
        // or
        // <AuthorizeView Policy="@Policies.IsAdmin"> // for page element attribute
        //     <p>Welcome Administrator</p>
        // </AuthorizeView>

        [DisplayName("Identity")]
        [Description("Identity Permissions")]
        public static class Identity
        {
            public const string Register = "Permissions.Identity.Register";
        }

        [DisplayName("Roles")]
        [Description("Roles Permissions")]
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string ListAll = "Permissions.Roles.ViewAll";
            public const string Register = "Permissions.Roles.Register";
            public const string Update = "Permissions.Roles.Update";
            public const string Remove = "Permissions.Roles.Remove";
        }

        [DisplayName("Products")]
        [Description("Products Permissions")]
        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Search = "Permissions.Products.Search";
            public const string Register = "Permissions.Products.Register";
            public const string Update = "Permissions.Products.Update";
            public const string Remove = "Permissions.Products.Remove";
        }

        [DisplayName("Brands")]
        [Description("Brands Permissions")]
        public static class Brands
        {
            public const string View = "Permissions.Brands.View";
            public const string Search = "Permissions.Brands.Search";
            public const string Register = "Permissions.Brands.Register";
            public const string Update = "Permissions.Brands.Update";
            public const string Remove = "Permissions.Brands.Remove";
        }

        [DisplayName("Role Claims")]
        [Description("Role Claims Permissions")]
        public static class RoleClaims
        {
            public const string View = "Permissions.RoleClaims.View";
            public const string Create = "Permissions.RoleClaims.Create";
            public const string Edit = "Permissions.RoleClaims.Edit";
            public const string Delete = "Permissions.RoleClaims.Delete";
            public const string Search = "Permissions.RoleClaims.Search";
        }
    }
}
