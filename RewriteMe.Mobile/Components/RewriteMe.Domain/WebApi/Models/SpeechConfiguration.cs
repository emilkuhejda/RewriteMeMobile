// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class SpeechConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the SpeechConfiguration class.
        /// </summary>
        public SpeechConfiguration()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the SpeechConfiguration class.
        /// </summary>
        public SpeechConfiguration(string subscriptionKey, string speechRegion, System.Guid audioSampleId, string subscriptionRemainingTimeString)
        {
            SubscriptionKey = subscriptionKey;
            SpeechRegion = speechRegion;
            AudioSampleId = audioSampleId;
            SubscriptionRemainingTimeString = subscriptionRemainingTimeString;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subscriptionKey")]
        public string SubscriptionKey { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "speechRegion")]
        public string SpeechRegion { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "audioSampleId")]
        public System.Guid AudioSampleId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "subscriptionRemainingTimeString")]
        public string SubscriptionRemainingTimeString { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (SubscriptionKey == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SubscriptionKey");
            }
            if (SpeechRegion == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SpeechRegion");
            }
            if (SubscriptionRemainingTimeString == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SubscriptionRemainingTimeString");
            }
            if (SubscriptionKey != null)
            {
                if (SubscriptionKey.Length > 50)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "SubscriptionKey", 50);
                }
            }
            if (SpeechRegion != null)
            {
                if (SpeechRegion.Length > 50)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "SpeechRegion", 50);
                }
            }
            if (SubscriptionRemainingTimeString != null)
            {
                if (SubscriptionRemainingTimeString.Length > 50)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "SubscriptionRemainingTimeString", 50);
                }
            }
        }
    }
}