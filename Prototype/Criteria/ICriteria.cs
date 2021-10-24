using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Criteria
{
    interface ICriteria
    {
        static string Name { get; }
        public double CalculateScore();
        public ProblemReport GenerateProblemReport();
    }
}
