using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Prototype.Evaluators
{
    //The Interface an Evaluator has to implement, so that the main program can run it in its evaluation-pipeline
    public interface IEvaluator
    {
        Task Evaluate(Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> complexities);
    }
}
