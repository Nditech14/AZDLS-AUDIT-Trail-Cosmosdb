using Cosmos.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Application.Services
{
    public class FileService<T> : IFileService<T>
    {
        private readonly IFileRepository<T> _fileRepository;

        public FileService(IFileRepository<T> fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<T> UploadFileAsync(Stream fileStream, string fileName)
        {
            return await _fileRepository.UploadFileAsync(fileStream, fileName);
        }

        public async Task<Stream> DownloadFileAsync(string fileId)
        {
            return await _fileRepository.DownloadFileAsync(fileId);
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            return await _fileRepository.DeleteFileAsync(fileId);
        }

        public async Task<IEnumerable<T>> GetFilesAsync()
        {
            return await _fileRepository.GetFilesAsync();
        }

    }
}
