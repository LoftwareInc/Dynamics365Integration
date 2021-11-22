using Newtonsoft.Json;

namespace NLApiClient.ResponsesMessages
{
    [JsonObject]
    public class CloudPrinterData
    {
        [JsonProperty]
        public string PrinterName { get; set; }

        [JsonProperty]
        public string DriverName { get; set; }

        [JsonProperty]
        public string SerialNumber { get; set; }

        [JsonProperty]
        public string Status { get; set; }
    }

}
