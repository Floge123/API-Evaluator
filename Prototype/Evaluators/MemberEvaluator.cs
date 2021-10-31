using Prototype.Criteria;
using System;
using System.Collections.Generic;
using System.Reflection;
using Prototype.ExtensionMethods;

namespace Prototype.Evaluators
{
    public class MemberEvaluator : IEvaluator
    {
        private Type[] assemblyType;
        private Dictionary<string, ICollection<ProblemReport>> problems;
        private Dictionary<string, double> complexities;

        public void Evaluate(
            Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> complexities)
        {
            this.assemblyType = assembly.GetExportedTypes();
            this.problems = problems;
            this.complexities = complexities;

            EvaluateMember();
        }

        private void EvaluateMember()
        {
            double countComplexity = 0.0;
            double prefixComplexity = 0.0;
            foreach(Type type in assemblyType)
            {
                ICriteria memberCountCriteria = new MemberCountCriteria(type);
                ICriteria memberPrefixCriteria = new MemberPrefixCriteria(type);
                countComplexity += memberCountCriteria.CalculateComplexity();
                prefixComplexity += memberPrefixCriteria.CalculateComplexity();
                problems.AddOrCreate(MemberCountCriteria.Name, memberCountCriteria.GenerateProblemReports());
                problems.AddOrCreate(MemberPrefixCriteria.Name, memberPrefixCriteria.GenerateProblemReports());
            }
            countComplexity /= assemblyType.Length;
            prefixComplexity /= assemblyType.Length;
            complexities.CreateOrIncrease(MemberCountCriteria.Name, countComplexity);
            complexities.CreateOrIncrease(MemberPrefixCriteria.Name, prefixComplexity);
        }
    }
}
