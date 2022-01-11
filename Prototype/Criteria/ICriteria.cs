using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prototype.Criteria
{
    public interface ICriteria
    {
        /// <summary>
        /// Name to identify the Criteria with
        /// </summary>
        static string Name { get; }
        
        /// <summary>
        /// Calculates the complexity as defined by the Criteria.
        /// </summary>
        /// <returns><see cref="Task"/> of <see cref="double"/></returns>
        Task<double> CalculateComplexity();
        
        /// <summary>
        /// Generates <see cref="ProblemReport"/> by comparing criteria complexities with defined thresholds.
        /// </summary>
        /// <returns></returns>
        Task<ICollection<ProblemReport>> GenerateProblemReports();
    }
}
