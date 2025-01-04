using BasketService.Services;

namespace BasketService.Jobs;

public class ReceivedProductUpdateMessage : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReceivedProductUpdateMessage(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        while (
            !stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IBasketService>();

            await orderService.GetMessageUpdateProductName();
        }
    }
}