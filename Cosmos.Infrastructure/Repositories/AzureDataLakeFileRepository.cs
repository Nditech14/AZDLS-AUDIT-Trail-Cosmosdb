using Azure.Storage.Files.DataLake;
using Cosmos.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cosmos.Infrastructure.Repositories
{
    public class AzureDataLakeFileRepository : IFileRepository<FileEntity>
    {
        private readonly DataLakeFileSystemClient _fileSystemClient;

        public AzureDataLakeFileRepository(string connectionString, string fileSystemName)
        {
            var serviceClient = new DataLakeServiceClient(connectionString);
            _fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            var fileClient = _fileSystemClient.GetFileClient(fileId);
            await fileClient.DeleteAsync();
            return true;
        }

        public async Task<Stream> DownloadFileAsync(string fileId)
        {
            var fileClient = _fileSystemClient.GetFileClient(fileId);
            var response = await fileClient.ReadAsync();
            return response.Value.Content;
        }

        public async Task<IEnumerable<FileEntity>> GetFilesAsync()
        {
            var files = new List<FileEntity>();
            await foreach (var pathItem in _fileSystemClient.GetPathsAsync())
            {
                files.Add(new FileEntity
                {
                    Id = pathItem.Name,
                    Name = pathItem.Name,
                    Url = _fileSystemClient.GetFileClient(pathItem.Name).Uri.AbsoluteUri
                });
            }
            return files;
        }

        public async Task<FileEntity> UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                var fileClient = _fileSystemClient.GetFileClient(fileName);

                // Ensure the file stream is at the start
                fileStream.Position = 0;

                // Upload the file stream, overwriting if it already exists
                await fileClient.UploadAsync(fileStream, overwrite: true);

                // Retrieve file properties to set the correct file details
                var properties = await fileClient.GetPropertiesAsync();

                return new FileEntity
                {
                    Id = fileClient.Path,
                    Name = fileClient.Name,
                    Url = fileClient.Uri.AbsoluteUri,
                    Size = properties.Value.ContentLength, 
                    ContentType = properties.Value.ContentType 
                };
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                Console.WriteLine($"Error uploading file to Azure Data Lake: {ex.Message}");
                throw;
            }
        }

    }
}
