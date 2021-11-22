using Newtonsoft.Json;

namespace NLApiClient.ResponsesMessages
{
    [JsonObject]
    public class ErrorResponse
    {
        [JsonProperty]
        public string Message { get; set; }
    }
}
