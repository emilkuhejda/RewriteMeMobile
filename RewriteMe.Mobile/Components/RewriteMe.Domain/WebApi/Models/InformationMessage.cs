// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class InformationMessage
    {
        /// <summary>
        /// Initializes a new instance of the InformationMessage class.
        /// </summary>
        public InformationMessage()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the InformationMessage class.
        /// </summary>
        public InformationMessage(System.Guid id, bool isUserSpecific, bool wasOpened, System.DateTime dateUpdatedUtc, System.DateTime datePublishedUtc, IList<LanguageVersion> languageVersions = default(IList<LanguageVersion>))
        {
            Id = id;
            IsUserSpecific = isUserSpecific;
            WasOpened = wasOpened;
            DateUpdatedUtc = dateUpdatedUtc;
            DatePublishedUtc = datePublishedUtc;
            LanguageVersions = languageVersions;
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
        [JsonProperty(PropertyName = "isUserSpecific")]
        public bool IsUserSpecific { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "wasOpened")]
        public bool WasOpened { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "dateUpdatedUtc")]
        public System.DateTime DateUpdatedUtc { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datePublishedUtc")]
        public System.DateTime DatePublishedUtc { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "languageVersions")]
        public IList<LanguageVersion> LanguageVersions { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (LanguageVersions != null)
            {
                foreach (var element in LanguageVersions)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
