using Cosmos.Application.Services;
using Cosmos.Core.Entities;
using Cosmos.Infrastructure.configuration;
using Cosmos.Infrastructure.Repositories;

namespace WebApi.Extension
{
    public static  class ExtendADLS
    {
        public static IServiceCollection AddAzureDataLakeServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the storage settings configuration section
            var storageSettings = configuration.GetSection("StorageSettings").Get<StorageSettings>();
            services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));

            // Register the Azure Data Lake file repository and related services
            services.AddScoped<IFileRepository<FileEntity>>(provider =>
                new AzureDataLakeFileRepository(storageSettings.ConnectionString, storageSettings.FileSystemName));

            // Register the file service
            services.AddScoped<IFileService<FileEntity>, FileService<FileEntity>>();

            return services;
        }
    }
}
