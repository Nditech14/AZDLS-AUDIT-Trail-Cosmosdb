using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.DTO
{
     public  class SubCategoryDto
    {
        [Required(ErrorMessage = "SubCategoryName is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "ProductName must be between 3 and 50 characters")]
        public string SubCategoryName { get; set; } = string.Empty;

    }
}
