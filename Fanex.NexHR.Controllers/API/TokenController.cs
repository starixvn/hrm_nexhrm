namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Fanex.NexHR.Models.Core;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    public class TokenController : ControllerBase
    {
        private readonly AuthorizedSettings authSettings;

        public TokenController(AuthorizedSettings authSettings)
        {
            this.authSettings = authSettings;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetToken(string username, string password)
        {
            if (CheckUser(username, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(authSettings.Secret);
                #pragma warning disable S6566 // Use "DateTimeOffset" instead of "DateTime"
                var expires = DateTime.UtcNow.AddMinutes(20);
                #pragma warning restore S6566 // Use "DateTimeOffset" instead of "DateTime"

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = expires,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var sercurityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(sercurityToken);

                return Ok(token);
            }

            return Unauthorized();
        }

        public bool CheckUser(string username, string password)
        {
            var isUsernamePasswordValid = password == authSettings.AppSecret
                && username == authSettings.AppId;

            return isUsernamePasswordValid;
        }
    }
}