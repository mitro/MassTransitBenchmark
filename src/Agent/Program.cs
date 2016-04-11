using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            NLogLogger.Use();

            ushort threadNum = 1000;

            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri($"rabbitmq://localhost"), h => { });

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
