using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.DTO
{
      public  class CategoryDto
    {
        [Required(ErrorMessage = "Category Name required")]
        [DataType(DataType.Text)]
        public string CategoryName { get; set; } = string.Empty;
        public List<SubCategoryDto> SubCategories { get; set; }
       
    }
}
