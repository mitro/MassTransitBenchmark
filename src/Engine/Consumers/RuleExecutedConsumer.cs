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

        public RuleExecutedConsumer(ContextStore contextStore)
        {
            _contextStore = contextStore;
        }

        public Task Consume(ConsumeContext<RuleExecuted> context)
        {
            var message = context.Message;

            var ctxId = message.ContextId;
            var ctx = _contextStore.Get(ctxId);
            ctx.Process(message);

            return Task.FromResult(0);
        }
    }
}