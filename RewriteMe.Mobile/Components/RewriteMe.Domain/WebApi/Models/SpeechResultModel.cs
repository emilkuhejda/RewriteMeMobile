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

    public partial class SpeechResultModel
    {
        /// <summary>
        /// Initializes a new instance of the SpeechResultModel class.
        /// </summary>
        public SpeechResultModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the SpeechResultModel class.
        /// </summary>
        public SpeechResultModel(System.Guid id, string totalTime)
        {
            Id = id;
            TotalTime = totalTime;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "totalTime")]
        public string TotalTime { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (TotalTime == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "TotalTime");
            }
        }
    }
}
