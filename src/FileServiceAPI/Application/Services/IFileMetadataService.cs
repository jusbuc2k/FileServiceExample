using FileServiceAPI.Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServiceAPI.Application.Services
{
    /// <summary>
    /// Represents a class, that when implemented, stores the metadata about files stored in the system.
    /// </summary>
    public interface IFileMetadataService
    {
        /// <summary>
        /// When implemented in a derived class, creates a file record with the given properties.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task<Record<FileProperties>> CreateFileAsync(FileCreationProperties properties);

        /// <summary>
        /// When implemented in a derived class, lists the files with the given criteria.
        /// </summary>
        /// <param name="name">If specified, searches for files containing the given name.</param>
        /// <param name="offset">If specified, starts returning files from the given offset (zero based).</param>
        /// <param name="limit">If specified, limits the response to the given number of records.</param>
        /// <returns></returns>
        Task<IEnumerable<Record<FileProperties>>> ListFilesAsync(string name = null, int offset = 0, int? limit = null);

        /// <summary>
        /// When implemented in a derived class, gets a file record by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Record<FileProperties>> GetFileAsync(Guid id);

        /// <summary>
        /// When implemented in a derived class, deletes the given file record by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteFileAsync(Guid id);

        /// <summary>
        /// When implemented in a derived class, sets the content-length property of the stored file record.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentLength"></param>
        /// <returns></returns>
        Task SetLengthAsync(Guid id, long contentLength);
    }
}
