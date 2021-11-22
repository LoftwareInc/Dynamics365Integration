using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLApiClient.ResponsesMessages;

namespace NLApiClient
{
    public class CloudPrintIntegrator : BaseIntegrator
    {
        private const string PrintersAPIPath = "Print/v1/Printers";
        private const string PrintAPIPath = "Print/v1/Print/";
        private const string StoreAPIPath = "Print/v1/Store/";
        private const string SendDataAPIPath = "Print/v1/SendData/";

        public CloudPrintIntegrator(string serviceURL)
            : base(serviceURL)
        {
        }

        public string GetPrinters(string subscriptionKey)
        {
            return GetPrintersAsync(subscriptionKey).GetAwaiter().GetResult();
        }

        public string[] GetPrintersArray(string subscriptionKey)
        {
            string result = GetPrinters(subscriptionKey);
            CloudPrinterData[] printers = JsonConvert.DeserializeObject<CloudPrinterData[]>(result);
            return printers.Select(x => x.PrinterName).OrderBy(x => x).ToArray<string>();
        }

        public string Print(string subscriptionKey, string printerName, string printRequestContent)
        {
            return PrintAsync(subscriptionKey, printerName, printRequestContent).GetAwaiter().GetResult();
        }

        public async Task<string> GetPrintersAsync(string subscriptionKey)
        {
            VerifyRequiredParameter(subscriptionKey, nameof(subscriptionKey));

            HttpClient client = GetAuthenticatedApiClient(subscriptionKey);
            string apiAddress = $"{BaseAPIUrl}{PrintersAPIPath}";
            HttpResponseMessage response = await client.GetAsync(apiAddress);

            string responseMessage = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responseMessage;
            }
            else
            {
                throw new Exception($"Reading printers failed: {responseMessage}");
            }
        }

        public async Task<string> PrintAsync(string subscriptionKey, string printerName, string printRequestContent)
        {
            VerifyRequiredParameter(subscriptionKey, nameof(subscriptionKey));
            VerifyRequiredParameter(printerName, nameof(printerName));

            HttpClient httpClient = GetAuthenticatedApiClient(subscriptionKey);
            string apiAddress = $"{BaseAPIUrl}{PrintAPIPath}{printerName}";

            // Set JSON request content.
            HttpRequestMessage putRequest = new HttpRequestMessage(HttpMethod.Put, apiAddress)
            {
                Content = new StringContent(printRequestContent, Encoding.UTF8, "application/json")
            };

            // Send the request and return unified response.
            ApiResponse responsesMessages = await PutApiRequest(httpClient, putRequest);
            return JsonConvert.SerializeObject(responsesMessages);
        }
    }
}
