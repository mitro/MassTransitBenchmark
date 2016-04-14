using DecisionEngine.Model;

namespace DecisionEngine.Api
{
    public class DecisionResponse
    {
        public ApplicationDecision ApplicationDecision { get; private set; }

        public DecisionResponse(ApplicationDecision applicationDecision)
        {
            ApplicationDecision = applicationDecision;
        }
    }
}