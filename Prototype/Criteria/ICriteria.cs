using System.Collections.Generic;

namespace Prototype.Criteria
{
    public interface ICriteria
    {
        public static string Name { get; }
        public double CalculateComplexity();
        public ICollection<ProblemReport> GenerateProblemReports();
    }
}
