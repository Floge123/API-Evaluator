using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.DataStructures;

namespace Prototype.Evaluators
{
    //The Interface an Evaluator has to implement, so that the main program can run it in its evaluation-pipeline
    public interface IEvaluator
    {
        Task<(Dictionary<string, ICollection<ProblemReport>> problems, Dictionary<string, double> complexities)> Evaluate(Assembly assembly);
    }
}
