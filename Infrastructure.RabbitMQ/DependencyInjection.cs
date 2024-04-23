using Infrastructure.RabbitMQ.Features;
using Infrastructure.RabbitMQ.Features.Core;
using Infrastructure.RabbitMQ.Features.Interface;
using Infrastructure.RabbitMQ.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMQ
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterRabbitMQ(this IServiceCollection services,IConfiguration configuration)
        {
            // get setting connect 
            services.Configure<RabbitOptions>(configuration.GetSection(RabbitOptions.SelectName));

            RabbitOptions options = configuration.GetSection(RabbitOptions.SelectName).Get<RabbitOptions>();

            // connect config
            services.AddSingleton<IConnectionFactory>(p => new ConnectionFactory()
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password
            });

            //manager
            services.AddSingleton<IRabbitManager, RabbitManager>();

            //sender
            services.AddSingleton<IRabbitMessageSender, RabbitMessageSender>();

            return services;
        }

        public static IServiceCollection ConnectRabbitMQ(this IServiceCollection services,
            Action<ExchangeManagerSetting> exchangeManagerConfiguration,
            Action<QueueManagerSetting> queueManagerConfiguration)
        {

            var messageManagerSettings = new ExchangeManagerSetting();
            exchangeManagerConfiguration.Invoke(messageManagerSettings);
            services.AddSingleton(messageManagerSettings);

            var queueSettings = new QueueManagerSetting();
            queueManagerConfiguration.Invoke(queueSettings);
            services.AddSingleton(queueSettings);

            return services;
        }

        public static IServiceCollection AddReceiver<TObject, TReceiver>(this IServiceCollection services) where TObject : class
            where TReceiver : class, IRabbitMessageReceiver<TObject>
        {
            services.AddHostedService<RabbitQueueListener<TObject>>();
            services.AddTransient<IRabbitMessageReceiver<TObject>, TReceiver>();

            return services;
        }
    }
}