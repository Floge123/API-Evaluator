using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Criteria
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
        private readonly Type type;

        public static string Name => "Complexity of Method Return Values";

        public ReturnValueCriteria(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Complexity increase by 7 for each returnValue != null.
        /// </summary>
        /// <returns>complexity of return values</returns>
        public async Task<double> CalculateComplexity()
        {
            var complexity = 0.0;
            foreach (var method in type.GetMethods())
            {
                if (method.ReturnType != typeof(void))
                {
                    complexity += ComplexityIncrease;
                }
            }
            return await Task.FromResult(complexity);
        }

        public async Task<ICollection<ProblemReport>> GenerateProblemReports()
        {
            return await Task.FromResult(new List<ProblemReport>());
        }
    }
}
