using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    //The Interface an Evaluator has to implement, so that the main program can run it in its evaluation-pipeline
    interface IEvaluator
    {
        public void Evaluate(Assembly assembly, Dictionary<Criteria, List<ProblemReport>> problems, Dictionary<Criteria, int> score);
    }
}
