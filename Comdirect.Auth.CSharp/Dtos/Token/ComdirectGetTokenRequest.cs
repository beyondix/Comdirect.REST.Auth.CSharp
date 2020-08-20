using Newtonsoft.Json;

namespace Comdirect.Auth.CSharp.Dtos
{
    public class ComdirectGetTokenRequest
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
    }
}
