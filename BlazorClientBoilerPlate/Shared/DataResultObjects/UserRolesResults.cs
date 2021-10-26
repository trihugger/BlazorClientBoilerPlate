using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClientBoilerPlate.Shared.DataResultObjects
{
    public class UserRolesResults
    {
        public UserRoleResult[] UserRoles { get; set; }
    }

    public class UserRoleResult
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Enabled { get; set; }
    }
}
