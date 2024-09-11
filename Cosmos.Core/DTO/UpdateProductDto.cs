using Cosmos.Application.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cosmos.Core.DTO
{
    public class UpdateProductDto
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "ProductName must be between 3 and 50 characters")]
        public string? ProductName { get; set; } = string.Empty;



        [StringLength(200, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 200 characters")]
        public string? Description { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        public Status? Status { get; set; }


    }
}
