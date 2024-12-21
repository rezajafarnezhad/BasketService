namespace BasketService.Domain.Entities;

public class Basket
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid? DiscountId { get; set; }
    public List<BasketItem> BasketItems { get; set; } = [];

    public Basket(string userId)
    {
        UserId = userId;
    }

    public void RemoveItem(Guid basketId, Guid itemId)
    {
        var basketItem = BasketItems.FirstOrDefault(c => c.BasketId == basketId && c.Id == itemId);
        BasketItems.Remove(basketItem);
    }

    public void SetQuantityBasketItem(Guid basketId, Guid itemId, int quantity)
    {
        var basketItem = BasketItems.FirstOrDefault(c => c.BasketId == basketId && c.Id == itemId);
        basketItem.SetQuantity(quantity);
    }

    public void ApplyDiscountToBasket(Guid discountId) => DiscountId = discountId;

    public void AddItem(int quantity, Guid basketId, Guid productId)
    {
        BasketItems.Add(new BasketItem(quantity, basketId, productId));
    }
}

public class BasketItem
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public int Quantity { get; set; }
    public Basket Basket { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    private BasketItem() { }
    public BasketItem(int quantity, Guid basketId, Guid productId)
    {
        Quantity = quantity;
        BasketId = basketId;
        ProductId = productId;
    }

    public void SetQuantity(int quantity) => Quantity = quantity;

}