using FileServiceAPI.Application.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;

namespace FileServiceAPI.Application.Services
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class InMemoryFileService : IFileBlobService, IFileMetadataService
    {
        private ConcurrentDictionary<Guid, Record<FileProperties>> _meta = new ConcurrentDictionary<Guid, Record<FileProperties>>();
        private ConcurrentDictionary<Guid, byte[]> _files = new ConcurrentDictionary<Guid, byte[]>();
        

        public Task<Record<FileProperties>> CreateFileAsync(FileCreationProperties properties)
        {
            var created = DateTimeOffset.UtcNow;
            var record = new Record<FileProperties>()
            {
                ID = Guid.NewGuid(),
                Created = created,
                Modified = created,
                Properties = new FileProperties()
                {
                    Name = properties.Name
                }
            };

            _meta.TryAdd(record.ID, record);

            return Task.FromResult(record);
        }

        public Task<Record<FileProperties>> GetFileAsync(Guid id)
        {
            if (_meta.TryGetValue(id, out var file))
            {
                return Task.FromResult(file);
            }

            return Task.FromResult<Record<FileProperties>>(null);
        }

        public Task<IEnumerable<Record<FileProperties>>> ListFilesAsync(string name = null, int offset = 0, int? limit = null)
        {
            var query = _meta.Values
                .OrderBy(o => o.Properties.Name)
                .AsQueryable();                

            if (name != null)
            {
                query = query.Where(x => x.Properties.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (offset > 0)
            {
                query = query.Skip(offset);
            }

            if (limit.HasValue && limit > 0)
            {
                query = query.Take(limit.Value);
            }

            return Task.FromResult<IEnumerable<Record<FileProperties>>>(query.ToList());
        }

        public Task SetLengthAsync(Guid id, long contentLength)
        {
            _meta.AddOrUpdate(id, (id) => { 
                throw new Exception("Record not found"); 
            }, (id, existing) =>
            {
                existing.Properties.ContentLength = contentLength;
                return existing;
            });

            return Task.CompletedTask;
        }

        public Task<Stream> DownloadFileAsync(Guid id)
        {
            if (_files.TryGetValue(id, out var buffer))
            {
                return Task.FromResult<System.IO.Stream>(new MemoryStream(buffer));
            }

            return Task.FromResult<Stream>(null);
        }

        public async Task<long> UploadFileAsync(Stream stream, Guid id)
        {
            using var buffer = new MemoryStream();

            await stream.CopyToAsync(buffer);

            var bytes = buffer.ToArray();

            _files.AddOrUpdate(id, bytes, (id, existing) => bytes);

            return bytes.LongLength;
        }

        Task IFileBlobService.DeleteFileAsync(Guid id)
        {
            _files.TryRemove(id, out _);

            return Task.CompletedTask;
        }

        Task IFileMetadataService.DeleteFileAsync(Guid id)
        {
            _meta.TryRemove(id, out _);

            return Task.CompletedTask;
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
