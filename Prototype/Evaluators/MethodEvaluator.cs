using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class MethodEvaluator : IEvaluator
    {
        private Type[] assemblyTypes;
        private Dictionary<Criteria, List<ProblemReport>> problems;
        private Dictionary<Criteria, int> scores;

        private FlagsAndScores flagsAndScores;
        public MethodEvaluator(FlagsAndScores flagsAndScores) {
            this.flagsAndScores = flagsAndScores;
        }

        public void Evaluate(Assembly assembly, Dictionary<Criteria, List<ProblemReport>> problems, Dictionary<Criteria, int> scores)
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
            foreach(Type type in assemblyTypes)
            {
                foreach (MethodInfo methodInfo in type.GetMethods())
                {
                    int paramCount = methodInfo.GetParameters().Length;
                    if (paramCount > flagsAndScores.ParameterCount_Flag_Ok)
                    {
                        var pr = new ProblemReport(
                            type.Name, methodInfo.ToString(),
                            "Number of Parameters wayyyy too large! Has {" + paramCount + "}.",
                            Criteria.ParameterCount, Extent.small, Extent.medium, "Why even try?"
                        );

                        Utils.UpdateDictionaries(
                            problems, scores, Criteria.ParameterCount,
                            pr, flagsAndScores.ParameterCount_Score_Bad
                        );                        
                    } 
                    else if (paramCount > flagsAndScores.ParameterCount_Flag_Good)
                    {
                        var pr = new ProblemReport(
                            type.Name, methodInfo.ToString(),
                            "Number of Parameters too large! Has {" + paramCount + "}.",
                            Criteria.ParameterCount, Extent.small, Extent.medium, "Try being better!"
                        );

                        Utils.UpdateDictionaries(
                            problems, scores, Criteria.ParameterCount,
                            pr, flagsAndScores.ParameterCount_Score_Ok
                        );
                    }
                }
            }
        }

        
    }
}
