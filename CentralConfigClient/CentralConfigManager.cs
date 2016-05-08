using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using CentralConfigClient.Models;

namespace CentralConfigClient
{
    /// <summary>
    /// A wrapper for the Centralconfig application configuration service.  More information here: 
    /// https://github.com/cagedtornado/centralconfig
    /// </summary>
    public class CentralConfigManager
    {
        /// <summary>
        /// The service url template
        /// </summary>
        private string _serviceUrlTemplate = "{0}{1}";

        /// <summary>
        /// The base api url for the service
        /// </summary>
        private string _baseServiceUrl = string.Empty;

        /// <summary>
        /// Creates a central config manager with the given server endpoint url
        /// </summary>
        /// <param name="serverUrl"></param>
        public CentralConfigManager(string serverUrl)
        {
            Uri uri = new Uri(serverUrl);
            _baseServiceUrl = uri.AbsoluteUri;
        }

        /// <summary>
        /// Get a list of applications
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigResponse<List<string>>> GetAllApplications()
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "applications/getall");

            //  Get the list of episodes
            var result = await MakeAPICall<ConfigResponse<List<string>>>(apiUrl);

            return result;
        }

        /// <summary>
        /// Get a list of applications
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigResponse<List<ConfigItem>>> GetAllConfigItems()
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "config/getall");

            //  Get the list of episodes
            var result = await MakeAPICall<ConfigResponse<List<ConfigItem>>>(apiUrl);

            return result;
        }

        #region API helpers

        /// <summary>
        /// Makes an API call and deserializes return value to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiCall"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        async private Task<T> MakeAPICall<T>(string apiCall, string postBody = "")
        {
            //  Make an async call to get the response
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PostAsync(apiCall, new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);

            //  Deserialize and return
            return (T)DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Deserializes the JSON string to the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objString"></param>
        /// <returns></returns>
        private T DeserializeObject<T>(string objString)
        {
            using(var stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        #endregion
    }
}
