using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorClient6test1.Shared.DataModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpireDate { get; set; }

        public User()
        {
            // TODO
        }

        public User(string token, string refreshtoken, DateTime expireDate)
        {
            Token = token;
            RefreshToken = refreshtoken;
            TokenExpireDate = expireDate;
        }

        public User(Guid id, string firstName, string lastName, string username, string token, string refreshtoken, DateTime expireDate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;    
            Username = username;
            Token = token;
            RefreshToken = refreshtoken;
            TokenExpireDate = expireDate;
        }
    }
}
