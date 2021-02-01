using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FileServiceAPI.Application.Domain
{
    /// <summary>
    /// Represents the properties that can be set on a new file.
    /// </summary>
    public class FileCreationProperties
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
    }
}
