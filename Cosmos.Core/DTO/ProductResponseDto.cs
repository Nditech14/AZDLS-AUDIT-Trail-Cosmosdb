using Cosmos.Application.Entities;
using Cosmos.Core.Entities;

namespace Cosmos.Core.DTO
{
    public class ProductResponseDto
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public List<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
        public int Inventory { get; set; }
        public decimal SalePrice { get; set; }
        public string Vendor { get; set; }
        public bool Published { get; set; }
        public Status Status { get; set; }

        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<FileEntity> MediaPaths { get; set; } = new List<FileEntity>();

    }
}
