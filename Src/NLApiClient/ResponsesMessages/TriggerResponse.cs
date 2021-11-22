using Newtonsoft.Json;

namespace NLApiClient.ResponsesMessages
{
    [JsonObject]
    public class TriggerResponse
    {
        [JsonProperty]
        public string Success { get; set; }

        [JsonProperty]
        public string Message { get; set; }
    }
}
