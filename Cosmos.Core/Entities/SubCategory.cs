using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Application.Entities
{
     public  class SubCategory
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string SubCategoryName { get; set; } = string.Empty;
        public string CategoryId { get; set; }
    }
}
