// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class LastUpdates
    {
        /// <summary>
        /// Initializes a new instance of the LastUpdates class.
        /// </summary>
        public LastUpdates()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the LastUpdates class.
        /// </summary>
        public LastUpdates(System.DateTime? fileItem = default(System.DateTime?), System.DateTime? transcribeItem = default(System.DateTime?), System.DateTime? userSubscription = default(System.DateTime?), System.DateTime? subscriptionProduct = default(System.DateTime?))
        {
            FileItem = fileItem;
            TranscribeItem = transcribeItem;
            UserSubscription = userSubscription;
            SubscriptionProduct = subscriptionProduct;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fileItem")]
        public System.DateTime? FileItem { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "transcribeItem")]
        public System.DateTime? TranscribeItem { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "userSubscription")]
        public System.DateTime? UserSubscription { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subscriptionProduct")]
        public System.DateTime? SubscriptionProduct { get; set; }

    }
}
