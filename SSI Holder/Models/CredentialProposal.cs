using Newtonsoft.Json;

namespace SSI_Holder.Models
{
    public class CredentialProposal
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; } = new();
    }
}
