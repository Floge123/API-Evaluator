using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.DataStructures;

namespace Prototype.Criteria.MethodScope
{
    /// <summary>
    /// A return value increase the complexity of a method by quite a lot.
    /// This increase, however only occurs if the return value is actually used.
    /// We can't know if the user will use it, so we measure assume a return value usage of around 87.5%.
    /// </summary>
    public class ReturnValueCriteria : ICriteria
    {
        /// <summary>
        /// Studies say that complexity increases by 8 if a return value is present and used.
        /// We assume 87.5% of return values are used, therefore use complexity of 7.
        /// </summary>
        private const int ComplexityIncrease = 7;
        private readonly MethodInfo _methodInfo;

        public ReturnValueCriteria(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        /// <summary>
        /// Complexity increase by 7 for each returnValue != null.
        /// </summary>
        /// <returns>complexity of return values</returns>
        public async Task<double> CalculateComplexity()
        {
            return await Task.FromResult(_methodInfo.ReturnType != typeof(void) ? ComplexityIncrease : 0);
        }

        public async Task<ICollection<ProblemReport>> GenerateProblemReports()
        {
            return await Task.FromResult(new List<ProblemReport>());
        }
    }
}
