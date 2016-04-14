using System.Threading.Tasks;

namespace DecisionEngine.Api
{
    public class DecisionEngineApiController
    {
        private readonly IDecisionEngine _engine;

        public DecisionEngineApiController(IDecisionEngine engine)
        {
            _engine = engine;
        }

        public async Task<DecisionResponse> Decide(DecisionRequest request)
        {
            var report = await _engine.DecideOn(request.Application);

            return new DecisionResponse(report);
        }
    }
}