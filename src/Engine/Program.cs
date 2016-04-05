using System;
using System.Diagnostics;
using System.Linq;
using Engine.Consumers;
using Engine.Contexts;
using Engine.Metrics;
using MassTransit;
using MassTransit.NLogIntegration.Logging;

namespace Engine
{
    class Program
    {
        private static int _contextCount;

        private static Stopwatch _stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            Console.Write("Enter number of contexts to start: ");
            _contextCount = int.Parse(Console.ReadLine());

            var contextStore = new ContextStore();
            var metricsStore = new MetricsStore(_contextCount);

            metricsStore.ExecutionCompleted += () =>
            {
                Console.WriteLine($"{metricsStore.FinishedContexts} contexts finished");
                Console.WriteLine($"{(metricsStore.FinishedContexts/_stopwatch.Elapsed.TotalSeconds):0} contexts/sec");
                Console.WriteLine($"{(metricsStore.ProcessedExecuteRule/_stopwatch.Elapsed.TotalSeconds):0} ExecuteRule/sec");
                Console.WriteLine($"{(metricsStore.ProcessedRuleExecuted/_stopwatch.Elapsed.TotalSeconds):0} RuleExecuted/sec");
                Console.WriteLine($"{Process.GetCurrentProcess().Threads.Count} threads used now");
                Console.WriteLine($"Total processing time {_stopwatch.Elapsed}");

                if (metricsStore.FinishedContexts == _contextCount)
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
                    e.Consumer(() => new RuleExecutedConsumer(contextStore, metricsStore));
                    e.Consumer(() => new ExecuteRuleConsumer(metricsStore));
                });
            });

            var busHandle = bus.Start();

            _stopwatch.Start();

            for (int i = 0; i < _contextCount; i++)
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
