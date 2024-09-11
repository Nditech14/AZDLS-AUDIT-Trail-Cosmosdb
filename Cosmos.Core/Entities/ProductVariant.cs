namespace Cosmos.Application.Entities
{
    public class ProductVariant
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string Variation { get; set; }
        public string SellerSKU { get; set; }
        public int Quantity { get; set; }
        public decimal CurrencyGlobalPrice { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public string ProductId { get; set; }
    }
}