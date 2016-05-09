using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace CentralConfigClient.Models
{
    [DataContract]
    public class ConfigItem
    {
        /// <summary>
        /// Unique id for the config item
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id
        { get; set; }

        /// <summary>
        /// The application name
        /// </summary>
        [DataMember(Name = "application")]
        public string Application
        { get; set; }

        /// <summary>
        /// The machine name
        /// </summary>
        [DataMember(Name = "machine")]
        public string Machine
        { get; set; }

        /// <summary>
        /// The config item name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        { get; set; }

        /// <summary>
        /// The config item value
        /// </summary>
        [DataMember(Name = "value", EmitDefaultValue = false)]
        public string Value
        { get; set; }

        /// <summary>
        /// The last updated time for the config item 
        /// (raw and unformatted)
        /// </summary>
        [DataMember(Name = "updated", EmitDefaultValue = false)]
        public string LastUpdatedRaw
        { get; set; }


        public DateTime LastUpdated
        {
            get
            {
                DateTime retval = DateTime.MinValue;

                //  Attempt to parse the LastUpdated date:
                try
                {                    
                    retval = DateTime.ParseExact(this.LastUpdatedRaw,
                                       "yyyy-MM-dd'T'HH:mm:ss'Z'",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeUniversal |
                                       DateTimeStyles.AdjustToUniversal);
                }
                catch { }

                //  Attempt to parse the LastUpdated date with milliseconds:
                if(retval == DateTime.MinValue)
                {
                    try
                    {
                        retval = DateTime.ParseExact(this.LastUpdatedRaw,
                                           "yyyy-MM-dd'T'HH:mm:ss.ff'Z'",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.AssumeUniversal |
                                           DateTimeStyles.AdjustToUniversal);
                    }
                    catch { }
                }
                
                return retval;
            }
        }
    }
}
