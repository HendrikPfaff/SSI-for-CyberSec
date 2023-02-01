using Newtonsoft.Json;

namespace SSI_Holder.Models
{
    public class Credential : BaseModel
    {
        [JsonProperty("referent")]
        public string Referent { get; set; }

        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        [JsonProperty("cred_def_id")]
        public string CredDefId { get; set; }

        [JsonProperty("rev_reg_id")]
        public string RevRegId { get; set; }

        [JsonProperty("cred_rev_id")]
        public string CredRevId { get; set; }

        [JsonProperty("attrs")]
        public Dictionary<string,string> Attrs { get; set; } = new();

        public List<KeyValuePair<string,string>> SortedAttrs
        {
            get => Attrs.OrderBy(x => x.Key).ToList();
            set => SortedAttrs = value;
        }

        public string DisplayName 
        {
            get => SchemaId.Split(':')[2].ToString(); 
            set => DisplayName = value; 
        }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    }
}