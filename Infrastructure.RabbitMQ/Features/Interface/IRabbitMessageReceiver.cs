namespace Infrastructure.RabbitMQ.Features.Interface
{
    public interface IRabbitMessageReceiver<T> where T : class
    {
        Task ReceiveAsync(T message, CancellationToken cancellationToken);
    }
}
