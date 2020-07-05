using Rockstars.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rockstars.Filter
{
    public class AuthenticateResponse
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public AuthenticateResponse(User user, string token)
        {
            Email = user.email;
            Token = token;
        }
    }
}
