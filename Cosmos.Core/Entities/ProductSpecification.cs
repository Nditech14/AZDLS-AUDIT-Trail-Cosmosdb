namespace Cosmos.Application.Entities
{
    public class ProductSpecification
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string Certifications { get; set; } = string.Empty;
        public string MainMaterials { get; set; } = string.Empty;
        public string MaterialFamily { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string ProductionCountry { get; set; } = string.Empty;
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string ProductId { get; set; } = string.Empty;
    }
}