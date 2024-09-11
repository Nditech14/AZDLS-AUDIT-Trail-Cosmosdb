using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Core.DTO
{
       public  class CategoryCreationDto
    {
        [Required(ErrorMessage = "CategoryName is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "CategoryName must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "CategoryName can only contain letters, numbers, and spaces")]
        public string CategoryName { get; set; } = string.Empty;

    }
}
