// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class RemainingTime
    {
        /// <summary>
        /// Initializes a new instance of the RemainingTime class.
        /// </summary>
        public RemainingTime()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RemainingTime class.
        /// </summary>
        public RemainingTime(long timeTicks)
        {
            TimeTicks = timeTicks;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timeTicks")]
        public long TimeTicks { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
