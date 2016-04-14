using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public class DecisionEngine : IDecisionEngine
    {
        private readonly IConfigProvider _configProvider;

        private readonly IContextCreator _contextCreator;

        private readonly IContextStore _contextStore;

        private readonly IContextRunnerRegistry _contextRunnerRegistry;

        public DecisionEngine(IConfigProvider configProvider, IContextCreator contextCreator, IContextStore contextStore, IContextRunnerRegistry contextRunnerRegistry)
        {
            _configProvider = configProvider;
            _contextCreator = contextCreator;
            _contextStore = contextStore;
            _contextRunnerRegistry = contextRunnerRegistry;
        }

        public async Task<ApplicationDecision> DecideOn(Application application)
        {
            var config = await _configProvider.GetConfig();

            var context = await _contextCreator.Create(application, config);

            await _contextStore.Save(context);

            var contextRunner = _contextRunnerRegistry.CreateAndRegister();

            try
            {
                return await contextRunner.RunToEnd(context);
            }
            finally
            {
                _contextRunnerRegistry.Unregister(contextRunner);
            }
        }
    }
}