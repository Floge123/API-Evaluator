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
        private const int COMPLEXITY_INCREASE = 7;
        private Type type;

        public static string Name { get { return "Complexity of Method Return Values"; } }
        public ReturnValueCriteria(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Complexity increase by 7 for each returnvalue != null.
        /// </summary>
        /// <returns>complexity of return values</returns>
        public double CalculateComplexity()
        {
            double complexity = 0.0;
            foreach (var method in type.GetMethods())
            {
                if (!method.ReturnType.Equals(typeof(void)))
                {
                    complexity += COMPLEXITY_INCREASE;
                }
            }
            return complexity;
        }

        public ICollection<ProblemReport> GenerateProblemReports()
        {
            return new List<ProblemReport>();
        }
    }
}
