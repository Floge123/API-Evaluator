using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.DataStructures;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria.MethodScope
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
        private readonly MethodInfo _methodInfo;
        private readonly int _paramCount;

        public ParamCountCriteria(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            _paramCount = methodInfo.GetParameters().Length;
        }

        /// <summary>
        /// The complexity increases exponentially by 2.5 per additional parameter
        /// </summary>
        /// <returns>complexity of method parameter count</returns>
        public async Task<double> CalculateComplexity()
        {
            return await Task.Run(() =>
            {
                double complexity = 0;
                for (var i = 1; i <= _paramCount; i++)
                {
                    complexity += 2.5 * i;
                }

                return complexity;
            });
        }

        public async Task<ICollection<ProblemReport>> GenerateProblemReports()
        {
            return await Task.Run(() =>
            {
                var problemReports = new List<ProblemReport>();
                if (_paramCount > FlagOk)
                {
                    problemReports.Add(new ProblemReport(
                        _methodInfo.DeclaringType?.Name, _methodInfo.Name,
                        $"Method has more than {FlagOk} parameters. Has {_paramCount}.",
                        nameof(ParamCountCriteria), "Reduce number of parameters or provide overload with less parameters."
                    ));
                }

                return problemReports;
            });
        }
    }
}
