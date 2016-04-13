using System;
using System.Collections.Generic;

namespace Engine.Contexts
{
    public interface IContextStore
    {
        void Insert(Context context);
        void Update(Context context);
        void AddExecutedRule(string contextId, Rule rule);
        void UpdateFinishedAt(string contextId, DateTime dateTime);
        Context Get(string contextId);
        Rule GetLastRuleExecuted(string contextId);
        IEnumerable<Context> All();
        void Clear();
    }
}