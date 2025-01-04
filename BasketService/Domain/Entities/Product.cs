namespace BasketService.Domain.Entities;

public class Product
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public string ImageProduct { get; set; }

    public Product(Guid productId, string productName, decimal unitPrice, string imageProduct)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        ImageProduct = imageProduct;
    }

    public void EditName(string productName) => ProductName = productName;
    public List<BasketItem> BasketItems { get; set; } = [];
}