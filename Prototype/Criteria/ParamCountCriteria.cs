using System;
using System.Collections.Generic;
using System.Reflection;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria
{
    /// <summary>
    /// To understand a method, the user has to understand the parameters.
    /// This complexity increases exponentially with increasing parameters.
    /// </summary>
    public class ParamCountCriteria : ICriteria
    {
        /// <summary>
        /// A method with more than 4 parameters is considered a problem.
        /// </summary>
        private const int FLAG_OK = 4;
        private Dictionary<string, int> paramCounts = new();
        private Type typeInfo;   

        public static string Name { get { return "Complexity of Method Parameter Counts"; } }

        public ParamCountCriteria(Type typeinfo)
        {
            this.typeInfo = typeinfo;
            foreach (var method in typeInfo.GetMethods())
            {
                paramCounts.CreateOrReplace(method.Name, method.GetParameters().Length);
            }
        }

        /// <summary>
        /// The complexity increases exponentially by 2.5 per additional parameter
        /// </summary>
        /// <returns>complexity of method parameter count</returns>
        public double CalculateComplexity()
        {
            double complexity = 0;
            foreach (var methods in paramCounts)
            {
                for (int i = 1; i <= methods.Value; i++)
                {
                    complexity += 2.5 * i;
                }
            }
            return complexity;
        }

        public ICollection<ProblemReport> GenerateProblemReports()
        {
            ICollection<ProblemReport> problemReports = new List<ProblemReport>();
            foreach (var methods in paramCounts)
            {
                if (methods.Value > FLAG_OK)
                {
                    problemReports.Add(new ProblemReport(
                        typeInfo.Name, methods.Key,
                        $"Method has more than {FLAG_OK} parameters. Has {methods.Value}.",
                        Name, "Reduce number of parameters or provide overload with less parameters."
                    ));
                }
            }
            
            return problemReports;
        }
    }
}
