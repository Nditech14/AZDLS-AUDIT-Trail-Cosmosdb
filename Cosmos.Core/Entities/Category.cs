using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Application.Entities
{
      public  class Category
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string CategoryName { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public List<SubCategory> subCategories { get; set; } = new List<SubCategory>();


    }
}
