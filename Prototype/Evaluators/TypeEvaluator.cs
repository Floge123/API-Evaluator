using Prototype.Criteria;
using Prototype.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Prototype.Evaluators
{
    public class TypeEvaluator : IEvaluator
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
            EvaluateCounts();
        }

        private void EvaluateCounts()
        {
            var typeCountComplexity = 0.0;
            var namespaceTypeCountComplexity = 0.0;
            var namespaceCountComplexity = 0.0;
            EvaluateCriteria(assemblyTypes, ref typeCountComplexity, TypeCountCriteria.Name, 
                assemblyTypes => new TypeCountCriteria(assemblyTypes));
            EvaluateCriteria(assemblyTypes, ref namespaceTypeCountComplexity, NamespaceTypeCountCriteria.Name, 
                assemblyTypes => new NamespaceTypeCountCriteria(assemblyTypes));
            EvaluateCriteria(assemblyTypes, ref namespaceCountComplexity, NamespaceCountCriteria.Name,
                assemblyTypes => new NamespaceCountCriteria(assemblyTypes));

            complexities.CreateOrIncrease(TypeCountCriteria.Name, typeCountComplexity);
            complexities.CreateOrIncrease(NamespaceTypeCountCriteria.Name, namespaceTypeCountComplexity);
            complexities.CreateOrIncrease(NamespaceCountCriteria.Name, namespaceCountComplexity);
        }

        private void EvaluateCriteria<TV>(Type[] types, ref double complexity, string name, Func<Type[], TV> ctor) where TV : ICriteria
        {
            ICriteria criteria = ctor.Invoke(types);
            complexity += criteria.CalculateComplexity();
            problems.AddOrCreate(name, criteria.GenerateProblemReports());
        }
    }
}
