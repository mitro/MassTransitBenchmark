using System;

namespace Contracts
{
    public class RuleExecuted
    {
        public DateTime CreatedAt { get; private set; }

        public int ContextId { get; set; }

        public RuleNumber Number { get; private set; }

        public RuleExecuted(DateTime createdAt, int contextId, RuleNumber number)
        {
            CreatedAt = createdAt;
            ContextId = contextId;
            Number = number;
        }
    }
}