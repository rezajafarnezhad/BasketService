using BasketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketService.Infrastructure;

public class BasketDatebaseContext : DbContext
{
    public BasketDatebaseContext(DbContextOptions<BasketDatebaseContext> options) : base(options) { }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
}