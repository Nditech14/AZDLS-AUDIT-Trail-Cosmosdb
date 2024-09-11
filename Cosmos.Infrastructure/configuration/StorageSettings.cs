using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Infrastructure.configuration
{
    public class StorageSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string FileSystemName { get; set; } = string.Empty;

    }
}
