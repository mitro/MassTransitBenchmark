using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
        private static ContextStore _contextStore;
        private static MetricsStore _metricsStore;

        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;

            Console.Write("Enter number of contexts to start: ");
            _contextCount = int.Parse(Console.ReadLine());

            _contextStore = new ContextStore();
            _metricsStore = new MetricsStore(_contextCount);

            _metricsStore.ExecutionCompleted += () =>
            {
                Console.WriteLine($"{_metricsStore.FinishedContexts} contexts finished");
                Console.WriteLine($"{(_metricsStore.FinishedContexts/_stopwatch.Elapsed.TotalSeconds):0} contexts/sec");
                Console.WriteLine($"{(_metricsStore.ProcessedExecuteRule/_stopwatch.Elapsed.TotalSeconds):0} ExecuteRule/sec");
                Console.WriteLine($"{(_metricsStore.ProcessedRuleExecuted/_stopwatch.Elapsed.TotalSeconds):0} RuleExecuted/sec");
                Console.WriteLine($"{Process.GetCurrentProcess().Threads.Count} threads used now");
                Console.WriteLine($"Total processing time {_stopwatch.Elapsed}");

                _stopwatch.Stop();
                Console.WriteLine($"Average context processing time {_contextStore.All().Average(c => c.ProcessingTimeInMs)} ms");
                Console.WriteLine($"Min context processing time {_contextStore.All().Min(c => c.ProcessingTimeInMs)} ms");
                Console.WriteLine($"Max context processing time {_contextStore.All().Max(c => c.ProcessingTimeInMs)} ms");
            };

            var bus = InitBus();

            var busHandle = bus.Start();

            _stopwatch.Start();

            Parallel.For(0, _contextCount, (i, s) =>
            {
                var context = new Context(bus);
                _contextStore.Add(context);
                context.ContextFinished += _metricsStore.LogContextFinish;
                context.Start();
            });

            Console.ReadLine();

            busHandle.Stop();
        }

        private static IBusControl InitBus()
        {
            NLogLogger.Use();

            ushort threadNum = 1000;

            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri($"rabbitmq://localhost"), h => { });

                x.ReceiveEndpoint(host, "Benchmark_Engine", e =>
                {
                    e.Consumer(() => new RuleExecutedConsumer(_contextStore, _metricsStore));

                    e.PrefetchCount = threadNum;
                });
            });

            return bus;
        }
    }
}
