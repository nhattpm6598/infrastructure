using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.RabbitMQ.Features.Core
{
    public interface IRabbitManager : IDisposable
    {
        IModel Channel { get;}

        void MarkAsComplete(BasicDeliverEventArgs message);

        void MarkAsRejected(BasicDeliverEventArgs message);
    }
}
