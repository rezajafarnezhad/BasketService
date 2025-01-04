using BasketService.Infrastructure;
using BasketService.Jobs;
using BasketService.MessagingBus;
using BasketService.MessagingBus.Models;
using BasketService.Services;
using DiscountService.Proto;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BasketDatebaseContext>(op =>
    op.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));


builder.Services.AddGrpcClient<DiscountServiceProto.DiscountServiceProtoClient>(service =>
{
    service.Address = new Uri(builder.Configuration["Discount:uri"]);
});

builder.Services.AddScoped<IBasketService, BasketService.Services.BasketService>();
builder.Services.AddScoped<IDiscountService, BasketService.Services.DiscountService>();
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddScoped<IMessageBus, RabbitMqMessageBus>();
builder.Services.AddScoped<IRabbitMqMessageBusHelper, RabbitMqMessageBusHelper>();

builder.Services.AddHostedService<ReceivedProductUpdateMessage>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
