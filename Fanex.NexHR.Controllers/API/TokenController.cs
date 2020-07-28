namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Web.API;
    using System.Configuration;
    using System.Net;
    using System.Web.Http;

    public class TokenController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult GetToken(string username, string password)
        {
            if (CheckUser(username, password))
            {
                return Ok(JwtManager.GenerateToken(username));
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        public static bool CheckUser(string username, string password)
        {
            var isUsernamePasswordValid = password == ConfigurationManager.AppSettings["NexHRPassword"]
                && username == ConfigurationManager.AppSettings["NexHRId"];

            return isUsernamePasswordValid;
        }
    }
}
