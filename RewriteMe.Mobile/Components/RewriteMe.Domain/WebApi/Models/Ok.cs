// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Ok
    {
        /// <summary>
        /// Initializes a new instance of the Ok class.
        /// </summary>
        public Ok()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Ok class.
        /// </summary>
        public Ok(System.DateTime dateTime)
        {
            DateTime = dateTime;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "dateTime")]
        public System.DateTime DateTime { get; private set; }

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
