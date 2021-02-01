using FileServiceAPI.Application.Domain;
using FileServiceAPI.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileServiceAPI.Controllers
{
    /// <summary>
    /// Provides methods for consuming the Files api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileBlobService _blobService;
        private readonly IFileMetadataService _metaData;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="metaData"></param>
        public FilesController(
            IFileBlobService blob,
            IFileMetadataService metaData
        )
        {
            _blobService = blob;
            _metaData = metaData;
        }

        /// <summary>
        /// Gets a list of all existing file metadata records.
        /// </summary>
        /// <param name="offset">The index at which to start returning records</param>
        /// <param name="limit">The maximum number of records to return</param>
        /// <param name="name">An optional search string for which files will be returned only when they contain the given string</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Record<FileProperties>>> Get(int offset = 0, int? limit = 50, string name = null)
        {
            return await _metaData.ListFilesAsync(name, offset, limit);
        }

        /// <summary>
        /// Gets a file metadata record by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Record<FileProperties>>> Get(Guid id)
        {
            var record = await _metaData.GetFileAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            return record;
        }

        /// <summary>
        /// Creates a file metadata record with the given properties.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<CreatedAtActionResult> Post([FromBody] FileCreationProperties properties)
        {
            var record = await _metaData.CreateFileAsync(properties);

            return this.CreatedAtAction(nameof(Get), new { id = record.ID }, record);;
        }

        /// <summary>
        /// Sets the content of the given file record by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{id}/Content")]
        public async Task<NoContentResult> PostContent(Guid id, IFormFile file)
        {
            using var inputStream = file.OpenReadStream();
            
            var contentLength = await _blobService.UploadFileAsync(inputStream, id);

            await _metaData.SetLengthAsync(id, contentLength);

            return NoContent();
        }

        /// <summary>
        /// Gets the file contents
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Content")]
        public async Task<ActionResult> GetContent(Guid id)
        {
            var file = await _metaData.GetFileAsync(id);
            var contentStream = await _blobService.DownloadFileAsync(id);

            return File(contentStream, "application/octet-stream", file.Properties.Name);
        }

        /// <summary>
        /// Deletes the given file metadata and content by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            // don't deal with "what if one of them fails" for now

            await _blobService.DeleteFileAsync(id);

            await _metaData.DeleteFileAsync(id);
        }
    }
}
