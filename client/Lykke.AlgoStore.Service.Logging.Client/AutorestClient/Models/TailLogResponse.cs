// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.Logging.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class TailLogResponse
    {
        /// <summary>
        /// Initializes a new instance of the TailLogResponse class.
        /// </summary>
        public TailLogResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the TailLogResponse class.
        /// </summary>
        public TailLogResponse(IList<string> log = default(IList<string>))
        {
            Log = log;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Log")]
        public IList<string> Log { get; set; }

    }
}
