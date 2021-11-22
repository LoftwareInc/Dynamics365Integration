using Newtonsoft.Json;

namespace NLApiClient.ResponsesMessages
{
    [JsonObject]
    public class ApiResponse
    {
        [JsonProperty]
        public bool Success { get; set; }

        [JsonProperty]
        public string Message { get; set; }
    }
}
