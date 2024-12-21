namespace BasketService.Services;
public class BasketModel
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid? DiscountId { get; set; }
    public List<BasketItemModel> BasketItems { get; set; } = [];

    public decimal Total()
    {
        if (BasketItems.Count > 0)
        {
            return BasketItems.Sum(c => c.Product.UnitPrice * c.Quantity);
        }

        return 0;
    }
}

public class BasketItemModel
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }

    public ProductModel Product { get; set; }
}

public class ProductModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public string ImageProduct { get; set; }
}

public class AddItemToBasketModel
{
    public Guid BasketId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ImageProduct { get; set; }
}
