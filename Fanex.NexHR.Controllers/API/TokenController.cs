namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    public class TokenController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetToken(string username, string password)
        {
            if (CheckUser(username, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var sercurityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(sercurityToken);

                return Ok(token);
            }

            return Unauthorized();
        }

        public static bool CheckUser(string username, string password)
        {
            var isUsernamePasswordValid = password == ConfigurationManager.AppSettings["NexHRPassword"]
                && username == ConfigurationManager.AppSettings["NexHRId"];

            return isUsernamePasswordValid;
        }
    }
}
