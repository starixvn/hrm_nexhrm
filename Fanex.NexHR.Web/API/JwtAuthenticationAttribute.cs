namespace Fanex.NexHR.Web.API
{
    using Fanex.NexHR.Resources;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;

    [AttributeUsage(AttributeTargets.All)]
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
            {
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult(Messages.MissingJwtToken, request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token).ConfigureAwait(false);

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult(Messages.InValidToken, request);
            }
            else
            {
                context.Principal = principal;
            }
        }

        private static bool ValidateToken(string token, out string username)
        {
            username = null;
            bool isValid;

            var simplePrinciple = JwtManager.GetPrincipal(token);
            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
            {
                isValid = false;
            }
            else
            {
                var usernameClaim = identity.FindFirst(ClaimTypes.Name);
                username = usernameClaim?.Value;
                isValid = !string.IsNullOrEmpty(username);
            }

            return isValid;
        }

        protected static Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            if (ValidateToken(token, out var username))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
            {
                parameter = "realm=\"" + Realm + "\"";
            }

            context.ChallengeWith("Bearer", parameter);
        }
    }
}