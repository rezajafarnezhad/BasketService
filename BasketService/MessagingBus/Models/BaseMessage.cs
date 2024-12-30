namespace BasketService.MessagingBus.Models
{

    public class RabbitMqConfiguration
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class BaseMessage
    {
        public Guid MessageId { get; set; } = new Guid();
        public DateTime MessageData { get; set; } = DateTime.Now;
    }
}
