using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine.Contexts
{
    public class MemoryContextStore : IContextStore
    {
        private readonly ConcurrentDictionary<string, Context> _contexts = new ConcurrentDictionary<string, Context>();

        public Task Insert(Context context)
        {
            if (!_contexts.TryAdd(context.Id, context)) throw new Exception($"Context with id = {context.Id} already exists");

            return Task.FromResult(0);
        }

        public Task AddExecutedRule(string contextId, Rule rule)
        {
            var context = Get(contextId);
            context.ExecutedRules.Add(rule);

            return Task.FromResult(0);
        }

        public Task UpdateFinishedAt(string contextId, DateTime dateTime)
        {
            var context = Get(contextId);
            context.FinishedAt = dateTime;

            return Task.FromResult(0);
        }

        public Task<Rule> GetLastRuleExecuted(string contextId)
        {
            var context = Get(contextId);
            return Task.FromResult(context.ExecutedRules.Last());
        }

        public Task<IEnumerable<Context>> All()
        {
            return Task.FromResult(_contexts.Select(c => c.Value));
        }

        public Task Clear()
        {
            _contexts.Clear();

            return Task.FromResult(0);
        }

        private Context Get(string contextId)
        {
            Context context;

            if (!_contexts.TryGetValue(contextId, out context)) throw new Exception($"Context with id={contextId} does not exist");

            return context;
        }
    }
}