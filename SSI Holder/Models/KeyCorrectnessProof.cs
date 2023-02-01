using Newtonsoft.Json;

namespace SSI_Holder.Models
{
    public class KeyCorrectnessProof
    {
        [JsonProperty("c")]
        public string C { get; set; }

        [JsonProperty("xy_cap")]
        public string XyCap { get; set; }

        [JsonProperty("xr_cap")]
        public List<List<string>> XrCap { get; set; } = new();

        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
