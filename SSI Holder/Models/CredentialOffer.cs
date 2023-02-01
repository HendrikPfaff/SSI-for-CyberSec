using Newtonsoft.Json;

namespace SSI_Holder.Models
{
    public class CredentialOffer
    {
        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        [JsonProperty("cred_def_id")]
        public string CredDefId { get; set; }

        [JsonProperty("key_correctness_proof")]
        public KeyCorrectnessProof KeyCorrectnessProof { get; set; }
    }
}
