// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class SubscriptionProduct
    {
        /// <summary>
        /// Initializes a new instance of the SubscriptionProduct class.
        /// </summary>
        public SubscriptionProduct()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the SubscriptionProduct class.
        /// </summary>
        public SubscriptionProduct(string id = default(string), string timeString = default(string))
        {
            Id = id;
            TimeString = timeString;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timeString")]
        public string TimeString { get; set; }

    }
}
