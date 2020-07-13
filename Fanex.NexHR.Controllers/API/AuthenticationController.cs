namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Web.Http;
    using Fanex.NexHR.Models;
    using Microsoft.IdentityModel.Tokens;

    public class AuthenticationController : ApiController
    {
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        [HttpPost]
        public IHttpActionResult Authenticate(string username, string password)
        {
            var isUsernamePasswordValid = password == ConfigurationManager.AppSettings["NexTaskSchedulePassword"]
                && username == ConfigurationManager.AppSettings["NexTaskScheduleId"];
            if (isUsernamePasswordValid)
            {
                var token = CreateToken(username);
                return Ok(token);
            }

            return Ok(new CommandResult { Code = 1, Message = null });
        }

        private static string CreateToken(string username)
        {
            var issuedAt = DateTime.UtcNow;
            var expires = issuedAt.AddMinutes(30);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            });

            var secrectKey = ConfigurationManager.AppSettings["NexTaskScheduleSecret"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secrectKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = tokenHandler.CreateJwtSecurityToken(
                BaseUrl,
                BaseUrl,
                claimsIdentity,
                issuedAt,
                expires,
                signingCredentials: signingCredentials);

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}