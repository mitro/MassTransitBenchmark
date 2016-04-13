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
        private readonly IContextStore _contextStore;
        private readonly ContextRunner _contextRunner;

        public RuleExecutedConsumer(IContextStore contextStore, ContextRunner contextRunner)
        {
            _contextStore = contextStore;
            _contextRunner = contextRunner;
        }

        public Task Consume(ConsumeContext<RuleExecuted> context)
        {
            var message = context.Message;

            var ctx = _contextStore.Get(message.ContextId);

            _contextRunner.Process(ctx, message);

            return Task.FromResult(0);
        }
    }
}