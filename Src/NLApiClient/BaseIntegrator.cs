using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLApiClient.ResponsesMessages;

namespace NLApiClient
{
    public abstract class BaseIntegrator
    {
        private string baseAPIUrl;

        public BaseIntegrator(string serviceURL)
        {
            // Slash added to avoid urls without path
            if (!serviceURL.EndsWith("/"))
            {
                serviceURL = serviceURL + "/";
            }

            baseAPIUrl = serviceURL;
        }

        protected string BaseAPIUrl
        {
            get
            {
                return this.baseAPIUrl;
            }
        }

        /// <summary>
        /// Put the API HTTP request and unify the error message.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        protected async Task<ApiResponse> PutApiRequest(HttpClient httpClient, HttpRequestMessage requestMessage)
        {
            // Send request.
            HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage);
            string responseMessage = await httpResponse.Content.ReadAsStringAsync();

            // Process response.
            bool success = httpResponse.StatusCode == System.Net.HttpStatusCode.OK;
            string response = string.Empty;

            try
            {
                if (success)
                {
                    response = JsonConvert.DeserializeObject<string>(responseMessage);
                }
                else
                {
                    // When request is not successful, received JSON API content can be in 2 different formats which are unified in following section.
                    try
                    {
                        ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(responseMessage);
                        response = error.Message;
                    }
                    catch (Exception)
                    {
                        response = JsonConvert.DeserializeObject<string>(responseMessage);
                    }

                }
            }
            catch (Exception)
            {
                // In case of unrecognized response format, raw string is returned.
                // It can often happen in case of cloud trigger call where user configuration of NiceLabel Automation can return anything.
                response = responseMessage;
            }

            // Return unified API response.
            return new ApiResponse()
            {
                Success = success,
                Message = response
            };
        }

        /// <summary>
        /// Generates HTTP client with authentication header and JSON response type
        /// </summary>
        /// <param name="subscriptionKey"></param>
        /// <returns>The Http Client.</returns>
        protected HttpClient GetAuthenticatedApiClient(string subscriptionKey)
        {
            HttpClient client = new HttpClient();

            // Add subscription key obtained from NiceLabel Developer portal.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Tell client that preferred response type is JSON.
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        /// <summary>
        /// Ensures that required parameters has value.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        protected void VerifyRequiredParameter(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new InvalidOperationException($"Call parameter: \"{parameterName}\" can not be empty.");
            }
        }
    }
}
