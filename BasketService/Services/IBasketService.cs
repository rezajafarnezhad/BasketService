using BasketService.Domain.Entities;
using BasketService.Infrastructure;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BasketService.Services;


public interface IBasketService
{
    Task<BasketModel> GetOrCreateBasketForUser(string userId);
    Task<BasketModel> GetBasketForUser(string userId);
    Task AddItemToBasketUser(AddItemToBasketModel addItem);
    Task RemoveItemToBasketUser(Guid basketId, Guid itemId);
    Task SetQuantityItemToBasketUser(Guid basketId, Guid itemId, int quantity);
    Task ApplyDiscountToBasket(Guid basketId, Guid discountId);
}

public class BasketService : IBasketService
{
    private readonly BasketDatebaseContext _context;
    public BasketService(BasketDatebaseContext context)
    {
        _context = context;
    }
    public async Task<BasketModel> GetOrCreateBasketForUser(string userId)
    {
        var basketUser = await _context.Baskets
            .Include(c => c.BasketItems).AsNoTracking().SingleOrDefaultAsync(c => c.UserId == userId);

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
        var basket = await _context.Baskets.Include(c => c.BasketItems)
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

        var item = addItem.Adapt(new BasketItem());
        basket.BasketItems.Add(item);
        await _context.SaveChangesAsync();

    }

    public async Task RemoveItemToBasketUser(Guid basketId, Guid itemId)
    {
        var basket = await _context.Baskets.Include(c => c.BasketItems).SingleOrDefaultAsync(c => c.Id == basketId);
        if (basket is null)
            throw new Exception("Basket not found ...");
        basket.RemoveItem(basket.Id, itemId);
        await _context.SaveChangesAsync();
    }

    public async Task SetQuantityItemToBasketUser(Guid basketId, Guid itemId, int quantity)
    {
        var basket = await _context.Baskets.Include(c => c.BasketItems).SingleOrDefaultAsync(c => c.Id == basketId);
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
}