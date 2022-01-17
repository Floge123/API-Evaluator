using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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
        private const int FlagOk = 4;
        private readonly Dictionary<string, int> paramCounts = new();
        private readonly Type typeInfo;   

        public static string Name => "Complexity of Method Parameter Counts";

        public ParamCountCriteria(Type typeInfo)
        {
            this.typeInfo = typeInfo;
            foreach (var method in typeInfo.GetMethods())
            {
                paramCounts.CreateOrReplace(method.Name, method.GetParameters().Length);
            }
        }

        /// <summary>
        /// The complexity increases exponentially by 2.5 per additional parameter
        /// </summary>
        /// <returns>complexity of method parameter count</returns>
        public async Task<double> CalculateComplexity()
        {
            double complexity = 0;
            foreach (var (_, count) in paramCounts)
            {
                for (var i = 1; i <= count; i++)
                {
                    complexity += 2.5 * i;
                }
            }
            return await Task.FromResult(complexity);
        }

        public async Task<ICollection<ProblemReport>> GenerateProblemReports()
        {
            var problemReports = new List<ProblemReport>();
            foreach (var (method, count) in paramCounts)
            {
                if (count > FlagOk)
                {
                    problemReports.Add(new ProblemReport(
                        typeInfo.Name, method,
                        $"Method has more than {FlagOk} parameters. Has {count}.",
                        Name, "Reduce number of parameters or provide overload with less parameters."
                    ));
                }
            }
            
            return await Task.FromResult(problemReports);
        }
    }
}
