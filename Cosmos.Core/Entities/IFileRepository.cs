using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.Entities
{
    public interface IFileRepository<T>
    {
        Task<T> UploadFileAsync(Stream fileStream, string fileName);
        Task<Stream> DownloadFileAsync(string fileId);
        Task<bool> DeleteFileAsync(string fileId);
        Task<IEnumerable<T>> GetFilesAsync();

    }
}
