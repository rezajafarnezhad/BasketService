using BasketService.Domain.Entities;
using BasketService.Infrastructure;
using BasketService.MessagingBus;
using BasketService.MessagingBus.Models;
using BasketService.Services.DiscountModel;
using BasketService.Services.MessageModels;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BasketService.Services;


public interface IBasketService
{
    Task<BasketModel> GetOrCreateBasketForUser(string userId);
    Task<BasketModel> GetBasketForUser(string userId);
    Task AddItemToBasketUser(AddItemToBasketModel addItem);
    Task RemoveItemToBasketUser(Guid basketId, Guid itemId);
    Task SetQuantityItemToBasketUser(Guid basketId, Guid itemId, int quantity);
    Task ApplyDiscountToBasket(Guid basketId, Guid discountId);
    Task<OperationResult> CheckOut(CheckOutBasketModel model, IDiscountService discountService);
}

public class BasketService : IBasketService
{
    private readonly BasketDatebaseContext _context;
    private readonly IMessageBus _messageBus;
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;
    public BasketService(BasketDatebaseContext context, IMessageBus messageBus, IOptions<RabbitMqConfiguration> rabbitMqConfiguration)
    {
        _context = context;
        _messageBus = messageBus;
        _rabbitMqConfiguration = rabbitMqConfiguration.Value;
    }
    public async Task<BasketModel> GetOrCreateBasketForUser(string userId)
    {
        var basketUser = await _context.Baskets
            .Include(c => c.BasketItems).ThenInclude(c => c.Product).AsNoTracking().SingleOrDefaultAsync(c => c.UserId == userId);

        if (basketUser is null)
        {
            var basket = new Basket(userId);
            _context.Add(basket);
            await _context.SaveChangesAsync();
            return new BasketModel()
            {
                Id = basket.Id,
                UserId = basket.UserId
            };
        }
        return basketUser.Adapt(new BasketModel());
    }
    public async Task<BasketModel> GetBasketForUser(string userId)
    {
        var basket = await _context.Baskets
            .Include(c => c.BasketItems)
            .ThenInclude(c => c.Product)
            .AsNoTracking().SingleOrDefaultAsync(c => c.UserId == userId);

        if (basket is null)
            return new BasketModel();

        return basket.Adapt(new BasketModel());
    }

    public async Task AddItemToBasketUser(AddItemToBasketModel addItem)
    {
        var basket = await _context.Baskets.FindAsync(addItem.BasketId);
        if (basket is null)
            throw new Exception("Basket not found ...");

        var isExistProduct = await _context.Products.AnyAsync(c => c.ProductId == addItem.ProductId);
        if (!isExistProduct)
        {
            var product = new Product(addItem.ProductId, addItem.ProductName, addItem.UnitPrice, addItem.ImageProduct);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }


        basket.AddItem(addItem.Quantity, addItem.BasketId, addItem.ProductId);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemToBasketUser(Guid basketId, Guid itemId)
    {
        var basket = await _context.Baskets
            .Include(c => c.BasketItems)
            .ThenInclude(c => c.Product)
            .SingleOrDefaultAsync(c => c.Id == basketId);

        if (basket is null)
            throw new Exception("Basket not found ...");
        basket.RemoveItem(basket.Id, itemId);
        await _context.SaveChangesAsync();
    }

    public async Task SetQuantityItemToBasketUser(Guid basketId, Guid itemId, int quantity)
    {
        var basket = await _context.Baskets
            .Include(c => c.BasketItems)
            .ThenInclude(c => c.Product)
            .SingleOrDefaultAsync(c => c.Id == basketId);
        if (basket is null)
            throw new Exception("Basket not found ...");
        basket.SetQuantityBasketItem(basket.Id, itemId, quantity);
        await _context.SaveChangesAsync();
    }

    public async Task ApplyDiscountToBasket(Guid basketId, Guid discountId)
    {
        var basket = await _context.Baskets.FindAsync(basketId);
        if (basket is null)
            throw new Exception("Basket not found ...");

        basket.ApplyDiscountToBasket(discountId);
        await _context.SaveChangesAsync();
    }

    public async Task<OperationResult> CheckOut(CheckOutBasketModel model, IDiscountService discountService)
    {
        //get basket
        var basket = await _context.Baskets.AsNoTracking()
            .Include(c => c.BasketItems)
            .ThenInclude(c => c.Product)
            .SingleOrDefaultAsync(c => c.Id == model.BasketId);

        if (basket is null)
            return OperationResult.NotFound("سبد خرید یافت نشد");

        //Create message
        var message = model.Adapt(new BasketCheckOutMessage());

        var totalPrice = 0;

        basket.BasketItems.ForEach(c =>
        {
            message.BasketItemMessage.Add(new BasketItemMessage()
            {
                ProductId = c.ProductId,
                ProductName = c.Product.ProductName,
                Quantity = c.Quantity,
                BasketItemId = c.Id,
                Price = c.Product.UnitPrice
            });
        });
        message.TotalPrice = message.BasketItemMessage.Sum(c => c.Quantity * c.Price);

        //get discount

        OperationResult<DiscountInfoModel> discount = null;
        if (basket.DiscountId is not null)
            discount = await discountService.GetDiscountById(basket.DiscountId.Value.ToString());

        //clc discount

        message.TotalPrice = discount is { Data: not null } ? message.TotalPrice - discount.Data.Amount : message.TotalPrice;
        //send message
        await _messageBus.SendMessage(message, _rabbitMqConfiguration.QueueName);



        _context.Baskets.Remove(basket);
        await _context.SaveChangesAsync();
        return OperationResult.Success();
    }
}