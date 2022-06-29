namespace Fanex.NexHR.Web.API
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        public AuthenticationHeaderValue Challenge { get; }

        public IHttpActionResult InnerResult { get; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized 
                && response.Headers.WwwAuthenticate.All(h => h.Scheme != Challenge.Scheme))
            {
                response.Headers.WwwAuthenticate.Add(Challenge);
            }

            return response;
        }
    }
}
