using Newtonsoft.Json;

namespace Comdirect.Auth.CSharp.Dtos
{
    public class ValidComdirectToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("kdnr")]
        public string Kdnr { get; set; }

        [JsonProperty("bpid")]
        public string Bpid { get; set; }

        [JsonProperty("kontaktId")]
        public string KontaktId { get; set; }
    }
}
