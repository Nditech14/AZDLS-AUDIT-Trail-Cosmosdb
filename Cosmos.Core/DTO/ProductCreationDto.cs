using Cosmos.Application.Entities;
using Cosmos.Core.Entities;

namespace Cosmos.Core.DTO
{
    public class ProductCreationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
        public List<ProductSpecificationDto> Specifications { get; set; } = new List<ProductSpecificationDto>();
        public int Inventory { get; set; }
        public decimal SalePrice { get; set; }
        public string Vendor { get; set; }
        public bool Published { get; set; }

        public Status Status { get; set; } = Status.Draft;

        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<FileEntity> MediaPaths { get; set; } = new List<FileEntity>();


    }
}
