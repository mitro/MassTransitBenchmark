using System;
using System.Configuration;
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
            _contextStore = new ContextStore();
            _metricsStore = new MetricsStore();

            var bus = InitBus();

            var busHandle = bus.Start();

            _metricsStore.ExecutionCompleted += () =>
            {
                Console.WriteLine($"{_metricsStore.FinishedContexts} contexts finished");
                Console.WriteLine($"{(_metricsStore.FinishedContexts/_stopwatch.Elapsed.TotalSeconds):0} contexts/sec");
                Console.WriteLine($"{Process.GetCurrentProcess().Threads.Count} threads used now");
                Console.WriteLine($"Total processing time {_stopwatch.Elapsed}");

                _stopwatch.Stop();
                Console.WriteLine($"Min context processing time {_contextStore.All().Min(c => c.ProcessingTimeInMs)} ms");
                Console.WriteLine($"Average context processing time {_contextStore.All().Average(c => c.ProcessingTimeInMs)} ms");
                Console.WriteLine($"Median context processing time {_contextStore.All().Select(c => c.ProcessingTimeInMs).Median()} ms");
                Console.WriteLine($"95 percentile context processing time {_contextStore.All().Select(c => c.ProcessingTimeInMs).Percentile(95)} ms");
                Console.WriteLine($"Max context processing time {_contextStore.All().Max(c => c.ProcessingTimeInMs)} ms");

                Console.WriteLine();

                //Start(bus);
            };

            Start(bus);

            busHandle.Stop();
        }

        private static void Start(IBusControl bus)
        {
            Console.Write("Enter number of contexts to start: ");
            _contextCount = int.Parse(Console.ReadLine());

            _contextStore.Clear();
            _metricsStore.Reset(_contextCount);
            _stopwatch.Restart();

            Parallel.For(0, _contextCount, (i, s) =>
            {
                var context = new Context(bus);
                _contextStore.Add(context);
                context.ContextFinished += _metricsStore.LogContextFinish;
                context.Start();
            });

            Console.ReadLine();
        }

        private static IBusControl InitBus()
        {
            var url = ConfigurationManager.AppSettings["RabbitmqUrl"];

            NLogLogger.Use();

            ushort threadNum = 1000;

            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri(url), h => { });

                x.ReceiveEndpoint(host, "Benchmark_Engine", e =>
                {
                    e.Consumer(() => new RuleExecutedConsumer(_contextStore));

                    e.PrefetchCount = threadNum;
                });
            });

            return bus;
        }
    }
}
