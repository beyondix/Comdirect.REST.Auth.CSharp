using Newtonsoft.Json;
using System.Collections.Generic;

namespace Comdirect.Auth.CSharp.Dtos
{
    public class ComdirectValidateSessionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("typ")]
        public string Typ { get; set; }

        [JsonProperty("challenge")]
        public string Challenge { get; set; }

        [JsonProperty("AvailableTypes")]
        public List<string> AvailableTypes { get; set; }
    }
}
