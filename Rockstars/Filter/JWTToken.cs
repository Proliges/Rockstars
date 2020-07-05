using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rockstars.Classes;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rockstars.Filter
{
    public class JWTToken
    {
        private IConfiguration configuration;

        public JWTToken()
        {

        }
        public JWTToken(IConfiguration _configure)
        {
            configuration = _configure;
        }

        public AuthenticateResponse Authenticate(string email)
        {
            User user = GetDummyUserToCompare(email);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }
        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
           // var test = configuration.GetChildren().ToList();
            var key = Encoding.ASCII.GetBytes("");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.email.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private User GetDummyUserToCompare(string email)
        {
            User user = new User { email = "rudi@jansen.nl" };
            if (email != user.email)
            {
                throw new Exception("bestaat niet ");
            }
            return user;
        }
    }
}
