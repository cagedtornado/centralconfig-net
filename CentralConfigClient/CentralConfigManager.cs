using System;
using System.Collections.Generic;
using System.IO;
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
        #region Properties and private members

        /// <summary>
        /// The service url template
        /// </summary>
        private string _serviceUrlTemplate = "{0}{1}";

        /// <summary>
        /// The base api url for the service
        /// </summary>
        private string _baseServiceUrl = string.Empty;


        private string _hostname = string.Empty;

        /// <summary>
        /// The hostname for the current machine
        /// </summary>
        public string HostName
        {
            get
            {
                if(_hostname == string.Empty)
                {
                    //  Lookup the current hostname
                    _hostname = Environment.MachineName;
                }

                return _hostname;
            }
        }

        #endregion

        #region Constructors

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
        /// Creates a central config manager with the given server endpoint and machine name
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="machineName"></param>
        public CentralConfigManager(string serverUrl, string machineName)
        {
            Uri uri = new Uri(serverUrl);
            _baseServiceUrl = uri.AbsoluteUri;
            _hostname = machineName;
        } 

        #endregion

        /// <summary>
        /// Get a list of applications
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigResponse<List<string>>> GetAllApplications()
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "applications/getall");

            //  Get the list of applications
            var result = await MakeAPICall<ConfigResponse<List<string>>>(apiUrl);

            return result;
        }

        /// <summary>
        /// Get a list of all config items
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigResponse<List<ConfigItem>>> GetAll()
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "config/getall");

            //  Get the list of config items
            var result = await MakeAPICall<ConfigResponse<List<ConfigItem>>>(apiUrl);

            return result;
        }

        /// <summary>
        /// Get a single config item
        /// </summary>
        /// <param name="application"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ConfigResponse<ConfigItem>> Get(string application, string name)
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "config/get");

            //  Construct the post body:
            string postBody = ToJSON(new ConfigItem()
            {
                Name = name,
                Application = application,
                Machine = this.HostName
            });

            //  Get a config item
            var result = await MakeAPICall<ConfigResponse<ConfigItem>>(apiUrl, postBody);

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

        /// <summary>
        /// Serialize the passed object (of type T) to a JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objItem"></param>
        /// <returns></returns>
        private string ToJSON<T>(T objData)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using(MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, objData);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        #endregion
    }
}
