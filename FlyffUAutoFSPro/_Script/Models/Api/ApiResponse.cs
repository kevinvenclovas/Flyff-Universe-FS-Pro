using Newtonsoft.Json;

namespace FlyffUAutoFSPro._Script.Models.Api
{
    public class ApiResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
