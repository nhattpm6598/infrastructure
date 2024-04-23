namespace Infrastructure.RabbitMQ.Features.Interface
{
    public interface IRabbitMessageSender
    {
        Task PublishAsync<T>(T message, int priority = 1) where T : class;

    }
}
