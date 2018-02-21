using CentralConfigClient.Models;
using FeatureFlags.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

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
                if(_hostname.Trim() == string.Empty)
                {
                    //  Lookup the current hostname
                    _hostname = Environment.MachineName;
                }

                return _hostname;
            }
        }

        /// <summary>
        /// The application to get/set information for
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// The current application user (used with feature flags to see if a feature is turned on for a user)
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The current application group (used with feature flags to see if a feature is turned on for a group)
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Is the current user / security context for an admin user (used with feature flags)
        /// </summary>
        public bool IsAdminUser { get; set; }

        /// <summary>
        /// Is the current user / security context for an internal user (used with feature flags)
        /// </summary>
        public bool IsInternalUser { get; set; }

        /// <summary>
        /// The current url (used with feature flags)
        /// </summary>
        public string Url { get; set; }

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

            User = "";
            Group = "";
            IsAdminUser = false;
            IsInternalUser = false;
        }

        /// <summary>
        /// Creates a central config manager with the given server endpoint, application and machine name
        /// </summary>
        /// <param name="serverUrl">The Centralconfig url</param>
        /// <param name="application">The application to get/set information for</param>
        /// <param name="machineName">Optional - Get configuration information for this machine name</param>
        public CentralConfigManager(string serverUrl, string application, string machineName = "") : this(serverUrl)
        {
            this.Application = application;
            _hostname = machineName;
        }

        /// <summary>
        /// Creates a central config manager with the given server endpoint, application and machine name and includes information for feature flags
        /// </summary>
        /// <param name="serverUrl">The Centralconfig url</param>
        /// <param name="application">The application to get/set information for</param>
        /// <param name="user">The current application user (for feature flags)</param>
        /// <param name="group">The current application group (for feature flags)</param>
        /// <param name="IsAdmin">Is the current user an admin user (for feature flags)</param>
        /// <param name="IsInternal">Is the current user an internal user (for feature flags)</param>
        /// <param name="machineName">Optional - Get configuration information for this machine name</param>
        public CentralConfigManager(string serverUrl, string application, string user, string group, bool IsAdmin, bool IsInternal, string machineName = "") : this(serverUrl, application, machineName)
        {
            User = user;
            Group = group;
            IsAdminUser = IsAdmin;
            IsInternalUser = IsInternal;
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
        /// Get a list of all config items for all applications
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
        /// Get a list of all config items for the given application (and the default app)
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public async Task<ConfigResponse<List<ConfigItem>>> GetAll(string application)
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "config/getallforapp");

            //  Construct the post body:
            string postBody = ToJSON(new ConfigItem()
            {
                Application = application
            });

            //  Get the list of config items
            var result = await MakeAPICall<ConfigResponse<List<ConfigItem>>>(apiUrl, postBody);

            return result;
        }

        /// <summary>
        /// Get a single config item
        /// </summary>
        /// <param name="name">The configuration key to get information for</param>
        /// <returns></returns>
        public async Task<ConfigResponse<ConfigItem>> Get(string name)
        {
            //  If we don't have an application set, throw an exception
            if(this.Application == string.Empty)
            {
                throw new ArgumentException("The calling application has not been set");
            }

            //  Otherwise, just call 'get':
            return await Get(this.Application, name);
        }

        /// <summary>
        /// Get a single config item
        /// </summary>
        /// <param name="application">The application to get configuration information for</param>
        /// <param name="name">The configuration key to get information for</param>
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

        /// <summary>
        /// Get a single config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the config value to fetch</param>
        /// <param name="defaultValue">The default value to return if the item can't be found</param>
        /// <returns></returns>
        public T Get<T>(string name, T defaultValue = default(T))
        {
            //  If we don't have an application set, throw an exception
            if(this.Application == string.Empty)
            {
                throw new ArgumentException("The calling application has not been set");
            }

            //  Otherwise, just call 'get':
            return Get<T>(this.Application, name, defaultValue);
        }

        /// <summary>
        /// Sees if a feature flag is enabled given the current app / user / group / etc
        /// </summary>
        /// <param name="name">The name of the feature flag to check</param>
        /// <returns></returns>
        public bool FeatureEnabled(string name)
        {
            bool retval = false;

            //  If we don't have an application set, throw an exception
            if (this.Application == string.Empty)
            {
                throw new ArgumentException("The calling application has not been set");
            }

            //  If we don't have a feature flag name, thrown an excaption:
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The feature flag name cannot be blank");
            }

            //  Get the base string value (we expect it to be a JSON encode string):
            string stringvalue = Get<string>(name);

            //  Parse the feature flag:
            FeatureFlag flag = stringvalue.ToFeatureFlag();

            //  See if the flag should be enabled given everything we know:
            retval = Feature.IsEnabled(flag, User ?? "", Group ?? "", Url ?? "", IsInternalUser, IsAdminUser);

            return retval;
        }

        /// <summary>
        /// Get a single config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="application">The application to get config information for</param>
        /// <param name="name">The name of the config value to fetch</param>
        /// <param name="defaultValue">The default value to return if the item can't be found</param>
        /// <returns></returns>
        public T Get<T>(string application, string name, T defaultValue = default(T))
        {
            T results = defaultValue;
            
            if(!string.IsNullOrEmpty(name))
            {
                try
                {
                    //  First get the string value:
                    string stringvalue = Get(application, name).Result.Data.Value;

                    //  Make sure the returned value isn't null or empty
                    if(!string.IsNullOrEmpty(stringvalue))
                    {
                        var theType = typeof(T);

                        //  If we're expecting an enumerated type...
                        if(theType.IsEnum)
                        {
                            //  attempt to cast to the enum:
                            results = (T)Enum.Parse(theType, stringvalue, true);
                        }
                        else
                        {
                            //  Otherwise, just cast to the type
                            results = (T)Convert.ChangeType(stringvalue, theType);
                        }
                    }
                }
                catch { /* Just fall through to the defaults */ }
            }

            return results;
        }

        /// <summary>
        /// Set a config item
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ConfigResponse<ConfigItem>> Set(ConfigItem data)
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "config/set");

            //  Construct the post body:
            string postBody = ToJSON(data);

            //  Get a config item
            var result = await MakeAPICall<ConfigResponse<ConfigItem>>(apiUrl, postBody);

            return result;
        }

        /// <summary>
        /// Remove a config item
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ConfigResponse<ConfigItem>> Remove(ConfigItem data)
        {
            //  Construct the url:
            string apiUrl = string.Format(_serviceUrlTemplate, _baseServiceUrl, "config/remove");

            //  Construct the post body:
            string postBody = ToJSON(data);

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
