using System;
using System.Threading.Tasks;
using Contracts;
using Engine.Contexts;
using Engine.Metrics;
using MassTransit;

namespace Engine.Consumers
{
    public class RuleExecutedConsumer: IConsumer<RuleExecuted>
    {
        private readonly ContextStore _contextStore;
        private readonly MetricsStore _metricsStore;

        public RuleExecutedConsumer(ContextStore contextStore, MetricsStore metricsStore)
        {
            _contextStore = contextStore;
            _metricsStore = metricsStore;
        }

        public Task Consume(ConsumeContext<RuleExecuted> context)
        {
            var message = context.Message;

            var ctxId = message.ContextId;
            var ctx = _contextStore.Get(ctxId);
            ctx.Process(message);

            _metricsStore.LogRuleExecutedProcessed();

            return Task.FromResult(0);
        }
    }
}