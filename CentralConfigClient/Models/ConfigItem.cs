using System;
using System.Runtime.Serialization;

namespace CentralConfigClient.Models
{
    [DataContract]
    public class ConfigItem
    {
        /// <summary>
        /// Unique id for the config item
        /// </summary>
        [DataMember(Name = "id")]
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
        [DataMember(Name = "value")]
        public string Value
        { get; set; }

        /// <summary>
        /// The last updated time for the config item
        /// </summary>
        [DataMember(Name = "updated")]
        public DateTime LastUpdated
        { get; set; }
    }
}
