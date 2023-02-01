using Newtonsoft.Json;

namespace SSI_Holder.Models
{
    public class Connection : BaseModel
    {
        [JsonProperty("invitation_mode")]
        public string InvitationMode { get; set; } = string.Empty;

        [JsonProperty("invitation_key")]
        public string InvitationKey { get; set; } = string.Empty;

        [JsonProperty("updated_at")] 
        public string UpdatedAt { get; set; } = string.Empty;

        [JsonProperty("their_did")]
        public string TheirDid { get; set; } = string.Empty;

        [JsonProperty("their_label")]
        public string TheirLabel { get; set; } = string.Empty;

        [JsonProperty("request_id")]
        public string RequestId { get; set; } = string.Empty;

        [JsonProperty("routing_state")]
        public string RoutingState { get; set; } = string.Empty;

        [JsonProperty("accept")]
        public string Accept { get; set; } = string.Empty;

        [JsonProperty("state")]
        public ConnectionState State { get; set; }

        [JsonProperty("initiator")]
        public string Initiator { get; set; } = string.Empty;

        [JsonProperty("connection_id")]
        public string ConnectionId { get; set; } = string.Empty;

        [JsonProperty("my_did")]
        public string MyDid { get; set; } = string.Empty;

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; } = string.Empty;
    }
}
