using Infrastructure.RabbitMQ.Common.Exceptions;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMQ.Options
{
#nullable disable

    public class ExchangeManagerSetting
    {
        public List<ExchangeSetting> Items { get; set; } = new();

        public void Add(string name, string type)
        {
            var list = ExchangeType.All();
            if (list.Contains(type))
            {
                Items.Add(new ExchangeSetting()
                {
                    Name = name,
                    Type = type
                });
            }
            else
            {
                throw new RabbitMQException(RabbitMQReason.EXCHANGE_TYPE_INCORRECT);
            }
        }
    }

    public class ExchangeSetting
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }

}
