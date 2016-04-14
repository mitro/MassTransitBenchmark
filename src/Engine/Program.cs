using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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

        private static readonly Stopwatch _stopwatch = new Stopwatch();

        private static IContextStore _contextStore;
        private static MetricsStore _metricsStore;
        private static ContextRunner _contextRunner;
        private static IBusControl _bus;
        private static BusHandle _busHandle;

        static void Main(string[] args)
        {
            InitDependencies();

            InitBus();

            AskContextCountAndStart().Wait();

            StopBus().Wait();
        }

        private static void InitBus()
        {
            var url = ConfigurationManager.AppSettings["RabbitMqUrl"];
            var username = ConfigurationManager.AppSettings["RabbitMqUsername"];
            var password = ConfigurationManager.AppSettings["RabbitMqPassword"];

            NLogLogger.Use();

            ushort threadNum = 100;

            _bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri(url), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                x.ReceiveEndpoint(host, "Benchmark_Engine", e =>
                {
                    e.Consumer(() => new RuleExecutedConsumer(_contextStore, _contextRunner));

                    e.PrefetchCount = threadNum;
                });
            });

            _contextRunner.Bus = _bus;

            _busHandle = _bus.Start();
        }

        private static void InitDependencies()
        {
            _contextStore = CreateContextStore();

            _metricsStore = new MetricsStore();
            _contextRunner = new ContextRunner(_contextStore);

            _contextRunner.ContextFinished += _metricsStore.LogContextFinish;

            _metricsStore.ExecutionCompleted += () =>
            {
                _stopwatch.Stop();

                Console.WriteLine($"{_metricsStore.FinishedContexts} contexts finished");
                Console.WriteLine($"{(_metricsStore.FinishedContexts/_stopwatch.Elapsed.TotalSeconds):0} contexts/sec");
                Console.WriteLine($"{Process.GetCurrentProcess().Threads.Count} threads used now");
                Console.WriteLine($"Total processing time {_stopwatch.Elapsed}");

                var processingTimes = _contextStore.All().Result.Select(c => c.ProcessingTimeInMs).ToArray();

                Console.WriteLine($"Min context processing time {processingTimes.Min()} ms");
                Console.WriteLine($"Average context processing time {processingTimes.Average()} ms");
                Console.WriteLine(
                    $"Median context processing time {processingTimes.Median()} ms");
                Console.WriteLine(
                    $"95 percentile context processing time {processingTimes.Percentile(95)} ms");
                Console.WriteLine($"Max context processing time {processingTimes.Max()} ms");

                Console.WriteLine();
            };
        }

        private static IContextStore CreateContextStore()
        {
            var contextStoreConfig = ConfigurationManager.AppSettings["ContextStore"];

            if (contextStoreConfig == "Memory")
            {
                return new MemoryContextStore();
            }

            if (contextStoreConfig == "MongoDb")
            {
                return new MongoContextStore();
            }

            throw new ConfigurationErrorsException("Invalid value of the ContextStore app setting");
        }

        private async static Task AskContextCountAndStart()
        {
            Console.Write("Enter number of contexts to start: ");
            _contextCount = int.Parse(Console.ReadLine());

            _metricsStore.ContextCount = _contextCount;

            _stopwatch.Restart();

            await _contextStore.Clear();

            Parallel.For(0, _contextCount, async (i, s) =>
            {
                await _contextRunner.Start(new Context());
            });

            Console.ReadLine();
        }

        private async static Task StopBus()
        {
            await _busHandle.StopAsync();
        }
    }
}
