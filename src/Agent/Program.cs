using System;
using System.Configuration;
using MassTransit;
using MassTransit.NLogIntegration.Logging;

namespace Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = InitBus();

            var busHandle = bus.Start();
        }

        private static IBusControl InitBus()
        {
            var url = ConfigurationManager.AppSettings["RabbitmqUrl"];
            var username = ConfigurationManager.AppSettings["Username"];
            var password = ConfigurationManager.AppSettings["Password"];

            NLogLogger.Use();

            ushort threadNum = 1000;

            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri(url), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                x.ReceiveEndpoint(host, "Benchmark_Agent", e =>
                {
                    e.Consumer(() => new ExecuteRuleConsumer());

                    e.PrefetchCount = threadNum;
                });
            });

            return bus;
        }
    }
}
