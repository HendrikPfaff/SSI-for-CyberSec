using Newtonsoft.Json;

namespace SSI_Holder.Models
{
    public class BaseModel
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
