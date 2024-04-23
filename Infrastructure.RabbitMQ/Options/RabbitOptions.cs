namespace Infrastructure.RabbitMQ.Options
{
#nullable disable
    public class RabbitOptions
    {
        public const string SelectName = "RabbitMQConfig";

        public string HostName{get;set;}

        public int Port{get;set;}

        public string Username{get;set;}

        public string Password { get; set; }

    }
}
