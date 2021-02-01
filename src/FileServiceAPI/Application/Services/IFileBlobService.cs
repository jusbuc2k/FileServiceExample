using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileServiceAPI.Application.Services
{
    /// <summary>
    /// Represents a class that when implemented, stores and retrieves file raw data.
    /// </summary>
    public interface IFileBlobService
    {
        /// <summary>
        /// When implemented in a derived class, uploads the contents of the given stream to the given file ID.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<long> UploadFileAsync(System.IO.Stream stream, Guid id);

        /// <summary>
        /// When implemented in a derived class, opens the content stream for the given file ID and returns it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<System.IO.Stream> DownloadFileAsync(Guid id);

        /// <summary>
        /// When implemented in a derived class, deletes the contents of the given file if it exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteFileAsync(Guid id);
    }
}
