using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Engine.Consumers;
using Engine.Contexts;
using Engine.Metrics;
using MassTransit;
using MassTransit.NLogIntegration.Logging;

namespace Engine
{
    class Program
    {
        private const int ContextCount = 1000;

        private static Stopwatch _stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            var contextStore = new ContextStore();
            var metricsStore = new MetricsStore(ContextCount);

            metricsStore.ExecutionCompleted += () =>
            {
                Console.Clear();
                Console.WriteLine($"{metricsStore.FinishedContexts} contexts finished");
                Console.WriteLine(
                    $"{(metricsStore.FinishedContexts/_stopwatch.Elapsed.TotalSeconds):0} contexts/sec");
                Console.WriteLine($"{Process.GetCurrentProcess().Threads.Count} threads used now");
                Console.WriteLine($"Total processing time {_stopwatch.Elapsed}");

                if (metricsStore.FinishedContexts == ContextCount)
                {
                    _stopwatch.Stop();
                    Console.WriteLine($"Average context processing time {contextStore.All().Average(c => c.ProcessingTimeInMs)} ms");
                    Console.WriteLine($"Min context processing time {contextStore.All().Min(c => c.ProcessingTimeInMs)} ms");
                    Console.WriteLine($"Max context processing time {contextStore.All().Max(c => c.ProcessingTimeInMs)} ms");
                }
            };

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

            _stopwatch.Start();

            for (int i = 0; i < ContextCount; i++)
            {
                var context = new Context(bus);
                contextStore.Add(context);
                context.ContextFinished += metricsStore.LogContextFinish;
                context.Start();
            }

            Console.ReadLine();

            busHandle.Stop();
        }
    }
}
