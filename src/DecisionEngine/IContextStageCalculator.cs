using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IContextStageCalculator
    {
        Task<ContextStageCalculationResult> Calculate(Context context);
    }
}