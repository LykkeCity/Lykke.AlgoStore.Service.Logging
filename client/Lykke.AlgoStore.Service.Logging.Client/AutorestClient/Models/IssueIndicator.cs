// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.Logging.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class IssueIndicator
    {
        /// <summary>
        /// Initializes a new instance of the IssueIndicator class.
        /// </summary>
        public IssueIndicator()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the IssueIndicator class.
        /// </summary>
        public IssueIndicator(string type = default(string), string value = default(string))
        {
            Type = type;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }
}
