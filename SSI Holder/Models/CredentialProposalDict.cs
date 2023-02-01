using Newtonsoft.Json;

namespace SSI_Holder.Models
{
    public class CredentialProposalDict
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("credential_proposal")]
        public CredentialProposal CredentialProposal { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("cred_def_id")]
        public string CredDefId { get; set; }

        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }
    }
}
