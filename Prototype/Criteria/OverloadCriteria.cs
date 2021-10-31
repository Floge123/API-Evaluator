using System;
using System.Collections.Generic;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria
{
    /// <summary>
    /// Member Overloading is a complexity-friendly way to increase functionaliy.
    /// However, Overloading still increases complexity and is therefore evaluated.
    /// We will not, however generate problems, as lots of overloads != bad design.
    /// /// </summary>
    public class OverloadCriteria : ICriteria
    {
        private const int FLAG_OK = 10;

        private Type type;
        private Dictionary<string, int> overloads = new();

        public static string Name { get { return "Complexity of Method Overloads"; } }
        public OverloadCriteria(Type type)
        {
            this.type = type;
            foreach (var method in type.GetMethods())
            {
                overloads.CreateOrIncrement(method.Name);
            }
        }

        /// <summary>
        /// Complexity increase by 1 for each overload found.
        /// </summary>
        /// <returns>complexity of overloads</returns>
        public double CalculateComplexity()
        {
            double complexity = 0.0;
            foreach (var overload in overloads)
            {
                if (overload.Value > 1)
                {
                    complexity += overload.Value;
                }
            }
            return complexity;
        }

        public ICollection<ProblemReport> GenerateProblemReports()
        {
            ICollection<ProblemReport> problems = new List<ProblemReport>();
            foreach (var overload in overloads)
            {
                if (overload.Value > 10)
                {
                    problems.Add(new ProblemReport(
                        type.Name, overload.Key,
                        $"Method has {overload.Value} overloads.",
                        Name, "This is just for info, no fix needed.")
                    );
                }
            }
            return problems;
        }
    }
}
