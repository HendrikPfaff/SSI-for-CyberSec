using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SSI_Holder.Models
{
    public class CredentialRecord : BaseModel
    {
        [JsonProperty("parent_thread_id")]
        public string ParentThreadId { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("credential")]
        public string Credential { get; set; }

        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }

        [JsonProperty("credential_proposal_dict")]
        public CredentialProposalDict CredentialProposalDict { get; set; }

        [JsonProperty("credential_exchange_id")]
        public string CredentialExchangeId { get; set; }

        [JsonProperty("trace")]
        public string Trace { get; set; }

        [JsonProperty("auto_remove")]
        public string AutoRemove { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("connection_id")]
        public string ConnectionId { get; set; }

        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        [JsonProperty("initiator")]
        public string Initiator { get; set; }

        [JsonProperty("auto_issue")]
        public string AutoIssue { get; set; }

        [JsonProperty("credential_offer")]
        public CredentialOffer CredentialOffer { get; set; }

        [JsonProperty("credential_request")]
        public string CredentialRequest { get; set; }

        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }

        [JsonProperty("credential_request_metadata")]
        public string CredentialRequestMetadata { get; set; }

        [JsonProperty("revoc_reg_id")]
        public string RevocRegId { get; set; }

        [JsonProperty("credential_definition_id")]
        public string CredentialDefinitionId { get; set; }

        [JsonProperty("credential_id")]
        public string CredentialId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("auto_offer")]
        public string AutoOffer { get; set; }

        [JsonProperty("raw_credential")]
        public string RawCredential { get; set; }

        [JsonProperty("revocation_id")]
        public string RevocationId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
