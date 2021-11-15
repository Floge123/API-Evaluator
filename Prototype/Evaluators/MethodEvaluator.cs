using Prototype.Criteria;
using Prototype.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Prototype.Evaluators
{
    public class MethodEvaluator : IEvaluator
    {
        private Type[] assemblyTypes;
        private Dictionary<string, ICollection<ProblemReport>> problems;
        private Dictionary<string, double> complexities;
        //criteria
        public void Evaluate(
            Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> complexities)
        {
            assemblyTypes = assembly.GetExportedTypes();
            this.problems = problems ?? throw new ArgumentNullException(nameof(problems));
            this.complexities = complexities ?? throw new ArgumentNullException(nameof(complexities));
            //call all private evaluations
            EvaluateMethods();
        }

        private void EvaluateMethods()
        {
            var count = 0;
            var paramComplexity = 0.0;
            var overloadComplexity = 0.0;
            var returnValueComplexity = 0.0;
            foreach(var type in assemblyTypes)
            {
                count += type.GetMethods().Length;
                EvaluateCriteria(type, ref paramComplexity, ParamCountCriteria.Name, type => new ParamCountCriteria(type));
                EvaluateCriteria(type, ref overloadComplexity, OverloadCriteria.Name, type => new OverloadCriteria(type));
                EvaluateCriteria(type, ref returnValueComplexity, ReturnValueCriteria.Name, type => new ReturnValueCriteria(type));
            }
            //parameter count complexity per method
            paramComplexity /= count;
            //overload complexity per type
            overloadComplexity /= assemblyTypes.Length;
            //return value complexity per method
            returnValueComplexity /= count;
            complexities.CreateOrIncrease(ParamCountCriteria.Name, paramComplexity);
            complexities.CreateOrIncrease(OverloadCriteria.Name, overloadComplexity);
            complexities.CreateOrIncrease(ReturnValueCriteria.Name, returnValueComplexity);
        }

        private void EvaluateCriteria<TV>(Type type, ref double complexity, string name, Func<Type, TV> ctor) where TV : ICriteria
        {
            ICriteria criteria = ctor.Invoke(type);
            complexity += criteria.CalculateComplexity();
            problems.AddOrCreate(name, criteria.GenerateProblemReports());
        }
    }
}
