using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLApiClient.ResponsesMessages;

namespace NLApiClient
{
    public class CloudTriggerIntegrator : BaseIntegrator
    {
        private const string PreviewTriggerName = "Api-CloudIntegrationDemo-Preview";
        private const string PrintTriggerName = "Api-CloudIntegrationDemo-Print";
        private const string GetPrintersTriggerName = "Api-CloudIntegrationDemo-Printers";
        private const string TriggerAPIPath = "Trigger/v1/CloudTrigger/";

        public CloudTriggerIntegrator(string serviceURL)
            : base(serviceURL)
        {
        }

        public string GetPreview(string subscriptionKey, string requestContent)
        {
            return TriggerAsync(subscriptionKey, PreviewTriggerName, requestContent, "application/json").GetAwaiter().GetResult();
        }

        public string Print(string subscriptionKey, string requestContent)
        {
            return TriggerAsync(subscriptionKey, PrintTriggerName, requestContent, "application/json").GetAwaiter().GetResult();
        }

        public string GetPrinters(string subscriptionKey)
        {
            return TriggerAsync(subscriptionKey, GetPrintersTriggerName, "{}", "application/json").GetAwaiter().GetResult();
        }

        public byte[] GetPreviewBytes(string subscriptionKey, string requestContent)
        {
            string result = GetPreview(subscriptionKey, requestContent);
            TriggerResponse triggerResponse = JsonConvert.DeserializeObject<TriggerResponse>(result);
            PreviewData previewData = JsonConvert.DeserializeObject<PreviewData>(triggerResponse.Message);
            return Convert.FromBase64String(previewData.Preview);
        }

        public string[] GetPrintersArray(string subscriptionKey)
        {
            string result = GetPrinters(subscriptionKey);
            TriggerResponse triggerResponse = JsonConvert.DeserializeObject<TriggerResponse>(result);
            CloudPrinterData[] printers = JsonConvert.DeserializeObject<CloudPrinterData[]>(triggerResponse.Message);
            return printers.Select(x => x.PrinterName).OrderBy(x => x).ToArray<string>();
        }

        public string Trigger(string subscriptionKey, string triggerName, string triggerContent, string contentType)
        {
            return TriggerAsync(subscriptionKey, triggerName, triggerContent, contentType).GetAwaiter().GetResult();
        }

        public async Task<string> TriggerAsync(string subscriptionKey, string triggerName, string stringRequestContent, string contentType)
        {
            VerifyRequiredParameter(subscriptionKey, nameof(subscriptionKey));
            VerifyRequiredParameter(triggerName, nameof(triggerName));

            HttpClient httpClient = GetAuthenticatedApiClient(subscriptionKey);
            string apiAddress = $"{BaseAPIUrl}{TriggerAPIPath}{triggerName}";

            // Prepare string request content
            HttpRequestMessage putRequest = new HttpRequestMessage(HttpMethod.Put, apiAddress)
            {
                Content = new StringContent(stringRequestContent, Encoding.UTF8, contentType)
            };

            // Send the request and return unified response.
            ApiResponse responsesMessages = await PutApiRequest(httpClient, putRequest);
            return JsonConvert.SerializeObject(responsesMessages);
        }

    }
}
