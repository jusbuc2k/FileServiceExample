using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FileServiceAPI.Application.Domain
{
    /// <summary>
    /// Represents the properties of a stored file.
    /// </summary>
    public class FileProperties : FileCreationProperties
    {
        /// <summary>
        /// Gets or sets the content length of the associated file contents.
        /// </summary>
        [JsonPropertyName("contentLength")]
        public long ContentLength { get; set; }
    }
}
