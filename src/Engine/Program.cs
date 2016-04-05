using System;
using System.Threading.Tasks;
using Contracts;
using Engine.Consumers;
using Engine.Contexts;
using MassTransit;
using MassTransit.NLogIntegration.Logging;

namespace Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            var contextStore = new ContextStore();

            NLogLogger.Use();

            var bus = Bus.Factory.CreateUsingInMemory(c =>
            {
                c.ReceiveEndpoint("engine_queue", e =>
                {
                    e.Consumer(() => new RuleExecutedConsumer(contextStore));
                    e.Consumer<ExecuteRuleConsumer>();
                });
            });

            var busHandle = bus.Start();

            Parallel.For(0, 99, (i, s) =>
            {
                var context = new Context(bus);
                contextStore.Add(context);
                context.Start();
            });
            
            Console.ReadLine();

            busHandle.Stop();
        }
    }
}
