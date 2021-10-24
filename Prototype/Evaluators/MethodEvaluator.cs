using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Prototype.ExtendMethods;
using Prototype.Criteria;

namespace Prototype
{
    class MethodEvaluator : IEvaluator
    {
        private Type[] assemblyTypes;
        private Dictionary<string, ICollection<ProblemReport>> problems;
        private Dictionary<string, double> scores;
        //criteria
        public void Evaluate(
            Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> scores)
        {
            //only Types are used here, so we don't have to save the whole assembly
            this.assemblyTypes = assembly.GetTypes();
            this.problems = problems;
            this.scores = scores;
            //call all private evaluation
            EvaluateParameterCount();
        }

        private void EvaluateParameterCount()
        {
            int count = 0;
            foreach(Type type in assemblyTypes)
            {
                foreach (MethodInfo methodInfo in type.GetMethods())
                {
                    //only work with public methods
                    if (methodInfo.IsPrivate) continue;
                    //delegate to criteria
                    count++;
                    ICriteria criteria = new ParamCountCriteria(type, methodInfo);
                    scores.AddOrCreate(ParamCountCriteria.Name, criteria.CalculateScore());
                    problems.AddOrCreate(ParamCountCriteria.Name, criteria.GenerateProblemReport());
                }
            }
            if (scores.ContainsKey(ParamCountCriteria.Name)) 
                scores[ParamCountCriteria.Name] /= count;

        }
    }
}
