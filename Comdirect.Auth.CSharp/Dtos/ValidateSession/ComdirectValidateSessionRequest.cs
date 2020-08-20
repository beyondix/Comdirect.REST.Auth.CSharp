using Newtonsoft.Json;

namespace Comdirect.Auth.CSharp.Dtos
{
    public class ComdirectValidateSessionRequest
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("sessionTanActive")]
        public bool SessionTanActive { get; set; } = true;

        [JsonProperty("activated2FA")]
        public bool Activated2FA { get; set; } = true;
    }
}
