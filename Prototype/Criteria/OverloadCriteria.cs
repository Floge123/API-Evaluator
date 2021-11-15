using System;
using System.Collections.Generic;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria
{
    /// <summary>
    /// Member Overloading is a complexity-friendly way to increase functionality.
    /// However, Overloading still increases complexity and is therefore evaluated.
    /// We will not, however generate problems, as lots of overloads != bad design.
    /// </summary>
    public class OverloadCriteria : ICriteria
    {
        private const int FlagOk = 10;

        private readonly Type type;
        private readonly Dictionary<string, int> overloads = new();

        public static string Name => "Complexity of Method Overloads";

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
            var complexity = 0.0;
            foreach (var (_, value) in overloads)
            {
                if (value > 1)
                {
                    complexity += value;
                }
            }
            return complexity;
        }

        public ICollection<ProblemReport> GenerateProblemReports()
        {
            ICollection<ProblemReport> problems = new List<ProblemReport>();
            foreach (var (key, value) in overloads)
            {
                if (value > FlagOk)
                {
                    problems.Add(new ProblemReport(
                        type.Name, key,
                        $"Method has {value} overloads.",
                        Name, "This is just for info, no fix needed.")
                    );
                }
            }
            return problems;
        }
    }
}
