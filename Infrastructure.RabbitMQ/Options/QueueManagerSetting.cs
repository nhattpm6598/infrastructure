namespace Infrastructure.RabbitMQ.Options
{
#nullable disable
    public class QueueManagerSetting
    {
        public List<QueueSetting> Items { get; set; } = new();

        public void Add<T>(QueueSetting message) where T : class
        {
            var type = typeof(T);
            Items.Add(new QueueSetting()
            {
                Name = message.Name ?? type.FullName,
                Type = type,
                Exchange = message.Exchange
            });
        }
    }

    public class QueueSetting
    {
        public virtual Type Type { get; set; }

        public string Name { get; set; }

        public string Exchange { get; set; }
    }

    public class BaseQueueMessage<T> : QueueSetting
        where T: class
    {
        public override Type Type { get ; set ; } = typeof(T);
    }

}
