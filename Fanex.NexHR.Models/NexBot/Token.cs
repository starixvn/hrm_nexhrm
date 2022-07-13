namespace Fanex.NexHR.Models.NexBot
{
    using Newtonsoft.Json;

    public class Token
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}