// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class RegisterUserModel
    {
        /// <summary>
        /// Initializes a new instance of the RegisterUserModel class.
        /// </summary>
        public RegisterUserModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RegisterUserModel class.
        /// </summary>
        public RegisterUserModel(string username = default(string), string password = default(string), string firstName = default(string), string lastName = default(string))
        {
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

    }
}
