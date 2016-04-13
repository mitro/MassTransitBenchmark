using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class ExecuteRule
    {
        public DateTime CreatedAt { get; private set; }

        public string ContextId { get; private set; }

        public RuleNumber Number { get; private set; }

        public ExecuteRule(DateTime createdAt, string contextId, RuleNumber number)
        {
            CreatedAt = createdAt;
            ContextId = contextId;
            Number = number;
        }
    }
}
