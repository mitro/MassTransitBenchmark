using System.Threading.Tasks;
using Contracts;
using Engine.Contexts;
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
            var ctxId = context.Message.ContextId;
            var ctx = _contextStore.Get(ctxId);
            ctx.Process(context.Message);
            return Task.FromResult(0);
        }
    }
}