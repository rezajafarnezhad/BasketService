using BasketService.MessagingBus.Models;

namespace BasketService.Services.MessageModels;

public class BasketCheckOutMessage : BaseMessage
{
    public Guid BasketId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Address { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PostalCode { get; set; }
    public string PhoneNumber { get; set; }
    public decimal TotalPrice { get; set; }
    public List<BasketItemMessage> BasketItemMessage { get; set; } = [];
}

public class BasketItemMessage
{
    public Guid BasketItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

}