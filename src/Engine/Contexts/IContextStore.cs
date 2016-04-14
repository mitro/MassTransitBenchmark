using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine.Contexts
{
    public interface IContextStore
    {
        Task Insert(Context context);
        Task AddExecutedRule(string contextId, Rule rule);
        Task UpdateFinishedAt(string contextId, DateTime dateTime);
        Task<Rule> GetLastRuleExecuted(string contextId);
        Task<IEnumerable<Context>> All();
        Task Clear();
    }
}