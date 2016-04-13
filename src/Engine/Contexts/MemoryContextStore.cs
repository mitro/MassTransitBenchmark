using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Contexts
{
    public class MemoryContextStore : IContextStore
    {
        private readonly ConcurrentDictionary<string, Context> _contexts = new ConcurrentDictionary<string, Context>();

        public void Insert(Context context)
        {
            if (!_contexts.TryAdd(context.Id, context)) throw new Exception($"Context with id = {context.Id} already exists");
        }

        public void AddExecutedRule(string contextId, Rule rule)
        {
            var context = Get(contextId);
            context.ExecutedRules.Add(rule);
        }

        public void UpdateFinishedAt(string contextId, DateTime dateTime)
        {
            var context = Get(contextId);
            context.FinishedAt = dateTime;
        }

        public Context Get(string contextId)
        {
            Context context;

            if (!_contexts.TryGetValue(contextId, out context)) throw new Exception($"Context with id={contextId} does not exist");

            return context;
        }

        public Rule GetLastRuleExecuted(string contextId)
        {
            var context = Get(contextId);
            return context.ExecutedRules.Last();
        }

        public IEnumerable<Context> All()
        {
            return _contexts.Select(c => c.Value);
        }

        public void Clear()
        {
            _contexts.Clear();
        }

        public void Update(Context context)
        {
            
        }
    }
}