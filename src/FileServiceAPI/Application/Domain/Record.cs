using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FileServiceAPI.Application.Domain
{
    /// <summary>
    /// Represents a record or object stored in the API.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Record<T>
    {
        /// <summary>
        /// Gets or sets the record's ID
        /// </summary>
        [JsonPropertyName("id")]
        [Required]
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the properties of the record.
        /// </summary>
        [JsonPropertyName("properties")]        
        public T Properties { get; set; }

        /// <summary>
        /// Gets or sets the date/time this record was created.
        /// </summary>
        [JsonPropertyName("created")]
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Gets or sets the date/time this record was last modified.
        /// </summary>
        [JsonPropertyName("modified")]
        public DateTimeOffset Modified { get; set; }
    }
}
