using BasketService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpPost("CreateOrGet")]
    public async Task<IActionResult> GetOrCreateBasketForUser(string userId)
    {
        var result = await _basketService.GetOrCreateBasketForUser(userId);
        return Ok(result);
    }
    [HttpGet("GetBasketForUser")]
    public async Task<IActionResult> GetBasketForUser(string userId)
    {
        var result = await _basketService.GetBasketForUser(userId);
        return Ok(result);
    }

    [HttpPost("AddItemToBasketUser")]
    public async Task<IActionResult> AddItemToBasketUser([FromForm] AddItemToBasketModel model)
    {
        await _basketService.AddItemToBasketUser(model);
        return NoContent();
    }

    [HttpDelete("RemoveItemToBasketUser{basketId}/{itemId}")]
    public async Task<IActionResult> RemoveItemToBasketUser(Guid basketId, Guid itemId)
    {
        await _basketService.RemoveItemToBasketUser(basketId, itemId);
        return NoContent();
    }

    [HttpPost("SetQuantityItemToBasketUser{basketId}/{itemId}/{quantity}")]
    public async Task<IActionResult> SetQuantityItemToBasketUser(Guid basketId, Guid itemId, int quantity)
    {
        await _basketService.SetQuantityItemToBasketUser(basketId, itemId, quantity);
        return NoContent();
    }
}