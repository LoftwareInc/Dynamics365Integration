using Newtonsoft.Json;

namespace NLApiClient.ResponsesMessages
{
    [JsonObject]
    public class PreviewData
    {
        [JsonProperty]
        public string Preview { get; set; }
    }
}
