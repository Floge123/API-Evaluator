﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Prototype.DataStructures;

namespace Prototype.Criteria
{
    public interface ICriteria
    {
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
